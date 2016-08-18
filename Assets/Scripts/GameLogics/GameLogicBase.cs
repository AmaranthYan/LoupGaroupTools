namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public abstract class GameLogicBase : PunBehaviour
    {
        public delegate void GameLogicCallback();
        protected static void DefaultGameLogicCallback() { }

        [SerializeField]
        protected CharacterDatabase m_CharacterDatabase = null;
        [SerializeField]
        protected PhotonView[] m_RuntimePhotonViews = new PhotonView[GameSessionService.ALLOCATED_PHOTON_VIEW_IDS_NUMBER - 1];

        public UnityTypedEvent.StringEvent onGameModeDescriptionInit = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onPlayerIdentitiesUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onUnusedIdentitiesUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.StringAndPlayerIdentityEvent onSinglePlayerIdentityUpdate = new UnityTypedEvent.StringAndPlayerIdentityEvent();
        public UnityTypedEvent.StringAndPlayerIdentityEvent onSingleUnusedIdentityUpdate = new UnityTypedEvent.StringAndPlayerIdentityEvent();

        protected PhotonPlayer[] m_Players = null;
        public int PlayerCount { get { return m_Players != null ? m_Players.Length : -1; } }
        protected GameModeModel m_GameMode = null;
        protected Dictionary<int, int> m_CharacterSet = null;
        public int[] CharacterIds { get { return m_CharacterSet.Keys.ToArray(); } }

        protected Dictionary<int, PlayerIdentity> m_PlayerIdentities = new Dictionary<int, PlayerIdentity>();
        public Dictionary<int, PlayerIdentity> PlayerIdentities { get { return m_PlayerIdentities; } }
        protected List<PlayerIdentity> m_UnusedIdentity = new List<PlayerIdentity>();

        protected bool m_IsInitialized = false;
        public bool IsInitialized { get { return m_IsInitialized; } }
        protected int m_CaptainNumber = -1;

        public GameLogicCallback InitGameLogic_Callback = DefaultGameLogicCallback;

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

            m_IsInitialized = true;

            InitGameLogic_Callback();
        }

        [PunRPC]
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

            m_UnusedIdentity.Clear();
            int identitiesCount = 0;
            foreach (int number in m_CharacterSet.Values)
            {
                identitiesCount += number;
            }
            for (int i = 0; i < identitiesCount - m_Players.Length + 1; i++)
            {
                PlayerIdentity unusedIdentity = new PlayerIdentity();
                unusedIdentity.UpdateNumber(i);
                m_UnusedIdentity.Add(unusedIdentity);
            }
            UpdateUnusedIdentities();
        }

        protected void UpdatePlayerIdentities()
        {
            List<int> playerNumbers = m_PlayerIdentities.Keys.ToList();
            playerNumbers.Sort();

            OrderedDictionary dictionary = new OrderedDictionary();
            for (int i = 0; i < playerNumbers.Count(); i++)
            {
                if (playerNumbers[i] != 0)
                {
                    dictionary.Add(playerNumbers[i].ToString(), m_PlayerIdentities[i]);
                }
            }
            onPlayerIdentitiesUpdate.Invoke(dictionary);
        }

        protected void UpdateUnusedIdentities()
        {
            OrderedDictionary dictionary = new OrderedDictionary();
            for (int i = 0; i < m_UnusedIdentity.Count(); i++)
            {
                dictionary.Add(i.ToString(), m_UnusedIdentity[i]);
            }
            onUnusedIdentitiesUpdate.Invoke(dictionary);
        }

        public abstract void RestartGame();
        public abstract void EndGame();
        public abstract void EndLocalGame();
        public abstract void GeneratePlayerIdentities();
        protected abstract void ReceivePlayerNumbers(int[] playerNumbers);
        public abstract void DistributePlayerIdentities();
        public abstract void ReversePlayerSurvivalStatus(int number);
        protected abstract void MarkPlayerAsDead(int number, bool isDead);
        public abstract void PromotePlayerToCaptain(int number);
        protected abstract void MarkPlayerAsCaptain(int number);
        public abstract void BroadcastPlayerIdentity(int number);
        public abstract void BroadcastUnusedIdentity(int index);
        protected abstract void ReceivePlayerIdentity(int number, int characterId);
        protected abstract void ReceiveUnusedIdentity(int index, int characterId);
    }
}
