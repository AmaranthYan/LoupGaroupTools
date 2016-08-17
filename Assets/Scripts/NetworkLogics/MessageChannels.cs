namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class MessageChannels : PunBehaviour
    {
        private class ChannelModel
        {
            private int m_ChannelId;
            private string m_ChannelName;
            private HashSet<int> m_PlayerNumbers;

            public int ChannelId { get { return m_ChannelId; } }
            public string ChannelName { get { return m_ChannelName; } set { m_ChannelName = value; } }
            public HashSet<int> PlayerNumbers { get { return m_PlayerNumbers; } set { m_PlayerNumbers = value; } }

            public ChannelModel(int id)
            {
                m_ChannelId = id;
                m_ChannelName = string.Empty;
                m_PlayerNumbers = new HashSet<int>();
            }
        }

        public const int MAX_CHANNEL_COUNT = 10;
        public const string ALL_CHANNEL_NAME = "ALL";

        [SerializeField]
        [Range(1,10)]
        private int m_DefaultChannelCount = 1;

        public UnityTypedEvent.OrderedDictionaryEvent onChannelsUpdate = new UnityTypedEvent.OrderedDictionaryEvent();

        private GameLogicBase m_GameLogic = null;

        private Dictionary<int, ChannelModel> m_ActiveChannels = new Dictionary<int, ChannelModel>();
        private HashSet<int> m_EmptyChannelIds = new HashSet<int>();
        //private Dictionary<int, List<int>> m_PlayerAvailableChannelList = new Dictionary<int, List<string>>();

        private List<ChannelModel> m_AvailableChannels = new List<ChannelModel>();
        private string m_CurrentChannelName = string.Empty;
        private string m_Message = string.Empty;

        #region Master
        void Awake()
        {
            m_GameLogic = FindObjectOfType<GameLogicBase>();
        }

        void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                InitChannels();
            }
        }

        private void InitChannels()
        {
            m_ActiveChannels = new Dictionary<int, ChannelModel>();
            for (int i = 0; i < MAX_CHANNEL_COUNT; i++)
            {
                if (i < m_DefaultChannelCount)
                {
                    m_ActiveChannels.Add(i, new ChannelModel(i));
                }
                else
                {
                    m_EmptyChannelIds.Add(i);
                }
            }
            //默认0号频道为全部玩家频道
            m_ActiveChannels[0].ChannelName = ALL_CHANNEL_NAME;
        }

        public void ResetChannels()
        {
            if (!PhotonNetwork.isMasterClient) { return; }
            //todo
            foreach (int id in m_ActiveChannels.Keys)
            {
                m_ActiveChannels[id].PlayerNumbers = new HashSet<int>();
            }

            //m_PlayerAvailableChannelList.Clear();
            ReleasePlayersFromChannels();

            UpdateChannels();
        }

        private void UpdateChannels()
        {            
            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (KeyValuePair<int, ChannelModel> channel in m_ActiveChannels)
            {
                dictionary.Add(channel.Key.ToString(), channel.Value);
            }
            onChannelsUpdate.Invoke(dictionary);
        }

        private void ReleasePlayersFromChannels()
        {
            photonView.RPC("UnsubscribeFromChannels", PhotonTargets.Others);
        }

        private void ReleasePlayersFromChannel(int channelId)
        {
            foreach (int playerNumber in m_ActiveChannels[channelId].PlayerNumbers)
            {
                photonView.RPC("UnsubscribeFromChannel", PhotonTargets.Others);
            }
        }

        public void AddChannel()
        {
            if (m_EmptyChannelIds.Count == 0) { return; }

            int id = m_EmptyChannelIds.First();
            if (m_EmptyChannelIds.Remove(id))
            {
                ChannelModel channel = new ChannelModel(id);
                m_ActiveChannels.Add(id, channel);

                UpdateChannels();
            }
        }

        public void DeleteChannel(int channelId)
        {
            //unsubscribe players

            //清除频道并回收ID
            ReleasePlayersFromChannel(channelId);
            m_ActiveChannels.Remove(channelId);
            m_EmptyChannelIds.Add(channelId);

            UpdateChannels();
        }
        #endregion

        #region OtherPlayers
        [PunRPC]
        private void UnsubscribeFromChannels()
        {
            m_AvailableChannels.Clear();

            ChannelModel channel = new ChannelModel(0);
            channel.ChannelName = ALL_CHANNEL_NAME;
            m_AvailableChannels.Add(channel);
        }

        public void SetCurrentChannelName(string channelName)
        {
            m_CurrentChannelName = channelName;
        }

        public void SetMessage(string msg)
        {
            m_Message = msg;
        }

        public void BroadcastMessageInCurrentChannel()
        {
            if (!m_AvailableChannels.Any(c => c.ChannelName == m_CurrentChannelName)) { return; }

            //todo
        }
        #endregion
    }
}
