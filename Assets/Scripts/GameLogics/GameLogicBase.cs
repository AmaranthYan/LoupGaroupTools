namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public abstract class GameLogicBase : PunBehaviour
    {
        protected delegate void GameLogicCallback();
        protected static void DefaultGameLogicCallback() { }

        [SerializeField]
        protected CharacterDatabase m_CharacterDatabase = null;
        [SerializeField]
        protected PhotonView[] m_RuntimePhotonViews = new PhotonView[GameSessionService.ALLOCATED_PHOTON_VIEW_IDS_NUMBER - 1];

        public UnityTypedEvent.StringEvent onGameModeDescriptionInit = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onPlayerIdentitiesUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onUnusedIdentitiesUpdate = new UnityTypedEvent.OrderedDictionaryEvent();

        protected PhotonPlayer[] m_Players = null;
        protected GameModeModel m_GameMode = null;
        protected Dictionary<int, int> m_CharacterSet = null;

        protected Dictionary<int, PlayerIdentity> m_PlayerIdentities = new Dictionary<int, PlayerIdentity>();
        protected List<CharacterModel> m_UnusedIdentity = new List<CharacterModel>();

        protected bool isInitialized = false;

        protected GameLogicCallback m_InitGameLogic_Callback = DefaultGameLogicCallback;

        public virtual void InitGameLogic(int[] allocatedPhotonViewIds, PhotonPlayer[] players, GameModeModel gameMode, Dictionary<int, int> characterSet)
        {
            for (int i = 0; i < m_RuntimePhotonViews.Length; i++)
            {
                if (m_RuntimePhotonViews[i])
                {
                    m_RuntimePhotonViews[i].viewID = allocatedPhotonViewIds[i];
                }
            }

            m_Players = players;
            m_GameMode = gameMode;
            onGameModeDescriptionInit.Invoke(m_GameMode.Description);
            m_CharacterSet = characterSet;

            GenerateEmptyIdenities();

            isInitialized = true;

            m_InitGameLogic_Callback();
        }

        protected void GenerateEmptyIdenities()
        {
            m_PlayerIdentities.Clear();

            for (int i = 0;i < m_Players.Length;i++)
            {
                PlayerIdentity playerIdentity = new PlayerIdentity();
                playerIdentity.UpdateNumber(i);
                m_PlayerIdentities.Add(i, playerIdentity);
            }

            UpdatePlayerIdentities();
        }

        protected void UpdatePlayerIdentities()
        {
            List<int> playerNumbers = m_PlayerIdentities.Keys.ToList();
            playerNumbers.Sort();

            OrderedDictionary dictionary = new OrderedDictionary();
            for (int i = 1; i < playerNumbers.Count(); i++)
            {
                dictionary.Add(i.ToString(), m_PlayerIdentities[i]);
            }
            onPlayerIdentitiesUpdate.Invoke(dictionary);
        }

        public abstract void GeneratePlayerIdentities();
        protected abstract void ReceivePlayerNumbers(int[] playerNumbers);
        public abstract void DistributePlayerCharacters();
        public abstract void BroadcastPlayerIdentity(PhotonPlayer player);
        protected abstract void ReceivePlayerIdentity(PhotonPlayer player, int characterId);
    }
}
