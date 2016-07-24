namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PlayerIdentity
    {
        private int m_Number;
        private PhotonPlayer m_Player;
        private int m_CharacterId;
        private bool m_IsDead;
        private bool m_IsCaptain;

        public int Number { get { return m_Number; } }
        public PhotonPlayer Player { get { return m_Player; } }
        public int CharacterId { get { return m_CharacterId; } }
        public bool IsDead { get { return m_IsDead; } }
        public bool IsCaptain { get { return m_IsCaptain; } }

        public PlayerIdentity()
        {
            m_Number = -1;
            m_Player = null;
            m_CharacterId = -1;
            m_IsDead = false;
            m_IsCaptain = false;
        }

        public void UpdateNumber(int num)
        {
            m_Number = num;
        }

        public void UpdatePlayer(PhotonPlayer player)
        {
            m_Player = player;
        }

        public void UpdateIdentity(int characterId)
        {
            m_CharacterId = characterId;
        }
    }
}
