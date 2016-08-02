namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    
    public class Vote : MonoBehaviour
    {
        private const float DEFAULT_VOTE_TIME_LIMIT = 10f;

        [SerializeField]
        private float m_VoteTimeLimit = DEFAULT_VOTE_TIME_LIMIT;

        private Dictionary<int, PlayerIdentity> m_PlayerIdentities = new Dictionary<int, PlayerIdentity>();

        void OnEnable()
        {
            RetrieveSurvivingPlayers();
        }

        public void ParseVoteTimeLimit(string timeString)
        {
            float timeLimit;
            m_VoteTimeLimit = float.TryParse(timeString, out timeLimit) ? timeLimit : DEFAULT_VOTE_TIME_LIMIT;
        }

        private void RetrieveSurvivingPlayers()
        {
            m_PlayerIdentities = FindObjectOfType<GameLogicBase>().PlayerIdentities;
        }

        public void Vote()
        {

        }
    }
}
