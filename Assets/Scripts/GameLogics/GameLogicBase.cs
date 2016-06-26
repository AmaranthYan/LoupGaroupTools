namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public abstract class GameLogicBase : PunBehaviour
    {
        private PhotonPlayer[] m_Players = null;
        private Dictionary<int, int> m_CharacterSet = null;

        public void InitGameLogic(PhotonPlayer[] players, Dictionary<int, int> characterSet)
        {
            m_Players = players;
            m_CharacterSet = characterSet;
        }
    }
}
