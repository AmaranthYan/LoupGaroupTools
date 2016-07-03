namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public abstract class GameLogicBase : PunBehaviour
    {
        protected delegate void GameLogicCallback();
        protected static void DefaultGameLogicCallback() { }

        [SerializeField]
        protected CharacterDatabase m_CharacterDatabase = null;

        protected PhotonPlayer[] m_Players = null;
        protected Dictionary<int, int> m_CharacterSet = null;

        protected Dictionary<int, PhotonPlayer> m_PlayerIdentities = new Dictionary<int, PhotonPlayer>();
        protected List<CharacterModel> m_UnusedCharacters = new List<CharacterModel>();

        protected bool isInitialized = false;

        protected GameLogicCallback m_InitGameLogic_Callback = DefaultGameLogicCallback;

        public virtual void InitGameLogic(PhotonPlayer[] players, Dictionary<int, int> characterSet)
        {
            m_Players = players;
            m_CharacterSet = characterSet;

            isInitialized = true;

            m_InitGameLogic_Callback();
        }

        public abstract void GeneratePlayerIdentities();
        public abstract void DistributePlayerIdentities();
        public abstract void BroadcastPlayerIdentity(PhotonPlayer player);
        protected abstract void ReceivePlayerIdentity(PhotonPlayer player, int characterId);
    }
}
