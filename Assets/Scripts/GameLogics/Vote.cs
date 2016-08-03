namespace LoupsGarous
{
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

        public UnityTypedEvent.OrderedDictionaryEvent onEligiblePlayersRetrieve = new UnityTypedEvent.OrderedDictionaryEvent();

        private Dictionary<int, PlayerIdentity> m_EligiblePlayers = new Dictionary<int, PlayerIdentity>();
        private List<PlayerIdentity> m_CandidateList = new List<PlayerIdentity>();
        private List<PlayerIdentity> m_VoterList = new List<PlayerIdentity>();

        public void ParseVoteTimeLimit(string timeString)
        {
            float timeLimit;
            m_VoteTimeLimit = float.TryParse(timeString, out timeLimit) ? timeLimit : DEFAULT_VOTE_TIME_LIMIT;
        }

        public void RetrieveEligiblePlayers()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            Dictionary<int, PlayerIdentity> playerIdentities = FindObjectOfType<GameLogicBase>().PlayerIdentities;
            m_EligiblePlayers = playerIdentities.Where(pi => (pi.Key != 0) && !pi.Value.IsDead);

            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_EligiblePlayers)
            {
                dictionary.Add(playerIdentity.Key.ToString(), playerIdentity.Value);
            }
            onEligiblePlayersRetrieve.Invoke(dictionary);
        }

        public void SetCandidateList(List<PlayerIdentity> candidateList)
        {
            m_CandidateList = candidateList;
        }

        public void SetVoterList(List<PlayerIdentity> voterList)
        {
            m_VoterList = voterList;
        }

        public void TakeVote()
        {
            if (!PhotonNetwork.isMasterClient) { return; }
            if (!m_CandidateList || !m_VoterList) { return; }

            //提取候选玩家编号
            int[] candidateNumbers = new int[m_CandidateList.Count];
            for (int i = 0; i < m_CandidateList.Count; i++)
            {
                candidateNumbers[i] = m_CandidateList[i].Number;
            }

            //向投票玩家发送候选玩家编号
            foreach (PlayerIdentity voter in m_VoterList)
            {
                photonView.RPC("CastVote", voter.Player, candidateNumbers);
            }
        }

        [PunRPC]
        private void CastVote(int[] candidateNumbers)
        {
            //todo
        }
    }
}
