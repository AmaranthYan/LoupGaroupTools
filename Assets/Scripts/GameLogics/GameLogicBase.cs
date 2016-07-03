namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public abstract class GameLogicBase : PunBehaviour
    {
        protected delegate void GameLogicCallback();

        [SerializeField]
        protected CharacterDatabase m_CharacterDatabase = null;

        protected PhotonPlayer[] m_Players = null;
        protected Dictionary<int, int> m_CharacterSet = null;

        protected Dictionary<int, PhotonPlayer> m_PlayerIdentities = new Dictionary<int, PhotonPlayer>();
        protected List<CharacterModel> m_UnusedCharacters = new List<CharacterModel>();

        protected GameLogicCallback m_InitGameLogic_Callback = null;

        public virtual void InitGameLogic(PhotonPlayer[] players, Dictionary<int, int> characterSet)
        {
            m_Players = players;
            m_CharacterSet = characterSet;

            m_InitGameLogic_Callback();
        }

        public abstract void GeneratePlayerIdentities();
        public abstract void DistributePlayerIdentities();
        public abstract void BroadcastPlayerIdentity(PhotonPlayer player);
        protected abstract void ReceivePlayerIdentity(PhotonPlayer player, int characterId);
    }
}
