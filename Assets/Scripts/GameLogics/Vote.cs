namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    [RequireComponent(typeof(PhotonView))]
    public class Vote : PunBehaviour
    {
        private const float DEFAULT_VOTE_TIME_LIMIT = 10f;

        [SerializeField]
        private float m_VoteTimeLimit = DEFAULT_VOTE_TIME_LIMIT;
        [SerializeField]
        private Animator m_CountdownAnimator = null;
        [SerializeField]
        private string m_CountdownParam = string.Empty;

        public UnityTypedEvent.OrderedDictionaryEvent onEligiblePlayersRetrieve = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onCandidatesRetrieve = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityEvent onVotePrepare = new UnityEvent();
        public UnityEvent onVoteStart = new UnityEvent();
        public UnityEvent onVoteTake = new UnityEvent();
        public UnityEvent onVoteCast = new UnityEvent();
        public UnityTypedEvent.StringEvent onCountdownUpdate = new UnityTypedEvent.StringEvent();
        public UnityEvent onCountdownEnd = new UnityEvent();
        public UnityEvent onPollDisplay = new UnityEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onPollUpdate = new UnityTypedEvent.OrderedDictionaryEvent();

        private GameLogicBase m_GameLogic = null;

        private Dictionary<int, PlayerIdentity> m_EligiblePlayers = new Dictionary<int, PlayerIdentity>();
        private Dictionary<int, PlayerIdentity> m_Candidates = new Dictionary<int, PlayerIdentity>();
        private List<PlayerIdentity> m_CandidateList = new List<PlayerIdentity>();
        private List<PlayerIdentity> m_VoterList = new List<PlayerIdentity>();
        private Dictionary<PlayerIdentity, bool> m_VoteChecklist = new Dictionary<PlayerIdentity, bool>();
        private Dictionary<int, int[]> m_Poll = new Dictionary<int, int[]>();
        private PlayerIdentity m_Vote = null;

        private IEnumerator m_VoteCountdown_Coroutine = null;

        void Awake()
        {
            m_GameLogic = FindObjectOfType<GameLogicBase>();
        }

        #region Master
        public void ParseVoteTimeLimit(string timeString)
        {
            float timeLimit;
            m_VoteTimeLimit = float.TryParse(timeString, out timeLimit) ? timeLimit : DEFAULT_VOTE_TIME_LIMIT;
        }

        public void SetCandidateList(List<PlayerIdentity> candidateList)
        {
            m_CandidateList = candidateList;
        }

        public void SetVoterList(List<PlayerIdentity> voterList)
        {
            m_VoterList = voterList;
        }

        private void RetrieveEligiblePlayers()
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;
            m_EligiblePlayers = playerIdentities.Where(pi => (pi.Key != 0) && !pi.Value.IsDead).ToDictionary(pi => pi.Key, pi => pi.Value);

            OrderedDictionary dictionary = new OrderedDictionary();
            if (m_EligiblePlayers != null) {
                foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_EligiblePlayers)
                {
                    dictionary.Add(playerIdentity.Key.ToString(), playerIdentity.Value);
                }
            }
            onEligiblePlayersRetrieve.Invoke(dictionary);
        }

        public void PrepareVote()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            RetrieveEligiblePlayers();
            onVotePrepare.Invoke();
        }

        public void StartVote()
        {
            if (!PhotonNetwork.isMasterClient) { return; }
            if ((m_CandidateList == null) || (m_VoterList == null)) { return; }

            //提取候选玩家编号
            int[] candidateNumbers = new int[m_CandidateList.Count];
            for (int i = 0; i < m_CandidateList.Count; i++)
            {
                candidateNumbers[i] = m_CandidateList[i].Number;
            }

            //向投票玩家发送候选玩家编号
            m_VoteChecklist.Clear();
            foreach (PlayerIdentity voter in m_VoterList)
            {
                photonView.RPC("TakeVote", voter.Player, m_VoteTimeLimit, candidateNumbers);
                m_VoteChecklist[voter] = false;
            }

            m_Poll.Clear();
            UpdatePoll();

            onVoteStart.Invoke();
        }

        [PunRPC]
        private void Poll(PhotonPlayer player, int vote)
        {
            //todo
            PlayerIdentity voter = m_VoteChecklist.Keys.FirstOrDefault(pi => pi.Player.Equals(player));
            if (voter.Equals(default(PlayerIdentity)) || m_VoteChecklist[voter]) { return; }

            m_VoteChecklist[voter] = true;
            List<int> voterList = m_Poll.ContainsKey(vote) ? m_Poll[vote].ToList() : new List<int>();
            voterList.Add(voter.Number);
            m_Poll[vote] = voterList.ToArray();

            UpdatePoll();
        }

        public void BroadcastPoll()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            photonView.RPC("DisplayPoll", PhotonTargets.Others);
            BroadcastPollResults();                
        }

        private void BroadcastPollResults()
        {
            foreach (KeyValuePair<int, int[]> result in m_Poll)
            {
                photonView.RPC("RetrievePollResult", PhotonTargets.Others, result.Key, result.Value);
            }

            photonView.RPC("UpdatePoll", PhotonTargets.Others);
        }
        #endregion

        #region All
        [PunRPC]
        private void UpdatePoll()
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;
            OrderedDictionary dictionary = new OrderedDictionary();
            //todo : m_Poll
            foreach (KeyValuePair<int, int[]> poll in m_Poll)
            {
                PlayerIdentity candidate = poll.Key != -1 ? playerIdentities[poll.Key] : null;
                List<PlayerIdentity> voters = new List<PlayerIdentity>();
                foreach (int voterNumber in poll.Value)
                {
                    voters.Add(playerIdentities[voterNumber]);
                }
                //将投票者按编号排序
                voters = voters.OrderBy(pi => pi.Number).ToList();
                dictionary.Add(poll.Key.ToString(), new DataPair<PlayerIdentity, List<PlayerIdentity>>(candidate, voters));
            }
            onPollUpdate.Invoke(dictionary);
        }
        #endregion

        #region OtherPlayers
        [PunRPC]
        private void TakeVote(float timeLimit, int[] candidateNumbers)
        {
            //清空之前的投票结果
            m_Poll.Clear();
            UpdatePoll();

            RetrieveCandidates(candidateNumbers);
            if (m_VoteCountdown_Coroutine != null)
            {
                StopCoroutine(m_VoteCountdown_Coroutine);
                m_VoteCountdown_Coroutine = null;
            }
            m_VoteCountdown_Coroutine = VoteCountdown(timeLimit);
            StartCoroutine(m_VoteCountdown_Coroutine);

            onVoteTake.Invoke();
        }

        private void RetrieveCandidates(int[] candidateNumbers)
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;
            m_Candidates = new Dictionary<int, PlayerIdentity>();
            foreach (int number in candidateNumbers)
            {
                m_Candidates[number] = playerIdentities[number];
            }

            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_Candidates)
            {
                dictionary.Add(playerIdentity.Key.ToString(), playerIdentity.Value);
            }
            onCandidatesRetrieve.Invoke(dictionary);
        }

        private IEnumerator VoteCountdown(float timeLimit)
        {
            float time = timeLimit;
            while (time > 0f)
            {
                onCountdownUpdate.Invoke(string.Format("{0:0.0}", time));
                m_CountdownAnimator.SetFloat(m_CountdownParam, time / timeLimit);
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            onCountdownEnd.Invoke();
        }

        public void SetVoteList(PlayerIdentity vote)
        {
            m_Vote = vote;
        }

        public void CastVote(bool isValid)
        {
            if (isValid && m_Vote == null) { return; }

            if (m_VoteCountdown_Coroutine != null)
            {
                StopCoroutine(m_VoteCountdown_Coroutine);
                m_VoteCountdown_Coroutine = null;
            }

            photonView.RPC("Poll", PhotonTargets.MasterClient, PhotonNetwork.player, isValid ? m_Vote.Number : -1);
            onVoteCast.Invoke();
        }

        [PunRPC]
        public void DisplayPoll()
        {
            onPollDisplay.Invoke();
        }

        [PunRPC]
        private void RetrievePollResult(int candidate, int[] voters)
        {
            m_Poll[candidate] = voters;
        }
        #endregion
    }
}
