namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
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
        [SerializeFiemd]
        private string m_CountdownParam = string.Empty;

        public UnityTypedEvent.OrderedDictionaryEvent onEligiblePlayersRetrieve = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onCandidatesRetrieve = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityEvent onVotePrepare = new UnityEvent();
        public UnityEvent onVoteStart = new UnityEvent();
        public UnityEvent onVoteTake = new UnityEvent();
        public UnityEvent onVoteCast = new UnityEvent();
        public UnityTypedEvent.StringEvent onCountdownUpdate = new UnityTypedEvent.StringEvent();
        public UnityEvent onCountdownEnd = new UnityEvent();

        private Dictionary<int, PlayerIdentity> m_EligiblePlayers = new Dictionary<int, PlayerIdentity>();
        private Dictionary<int, PlayerIdentity> m_Candidates = new Dictionary<int, PlayerIdentity>();
        private List<PlayerIdentity> m_CandidateList = new List<PlayerIdentity>();
        private List<PlayerIdentity> m_VoterList = new List<PlayerIdentity>();
        private PlayerIdentity m_Vote = null;

        private IEnumerator m_VoteCountdown_Coroutine = null;

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
            Dictionary<int, PlayerIdentity> playerIdentities = FindObjectOfType<GameLogicBase>().PlayerIdentities;
            m_EligiblePlayers = playerIdentities.Where(pi => (pi.Key != 0) && !pi.Value.IsDead) as Dictionary<int, PlayerIdentity>;

            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_EligiblePlayers)
            {
                dictionary.Add(playerIdentity.Key.ToString(), playerIdentity.Value);
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
            foreach (PlayerIdentity voter in m_VoterList)
            {
                photonView.RPC("TakeVote", voter.Player, m_VoteTimeLimit, candidateNumbers);
            }

            onVoteStart.Invoke();
        }

        [PunRPC]
        private void Poll(PhotonPlayer voter, int vote)
        {
            //todo
        }
        #endregion

        #region OtherPlayers
        [PunRPC]
        private void TakeVote(float timeLimit, int[] candidateNumbers)
        {
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
            Dictionary<int, PlayerIdentity> playerIdentities = FindObjectOfType<GameLogicBase>().PlayerIdentities;
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

        public void SetVoterList(PlayerIdentity vote)
        {
            m_Vote = vote;
        }

        public void CastVote()
        {
            if (m_VoteCountdown_Coroutine != null)
            {
                StopCoroutine(m_VoteCountdown_Coroutine);
                m_VoteCountdown_Coroutine = null;
            }

            photonView.RPC("Poll", PhotonTargets.MasterClient, PhotonNetwork.player, m_Vote.Number);
        }
        #endregion
    }
}
