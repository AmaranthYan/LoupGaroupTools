namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
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
        public const string ALL_CHANNEL_NAME = "全体玩家";
        public const string CHANNEL_DISPLAY_FORMAT = "{{0}}";
        public const string PLAYER_NUMBER_DISPLAY_FORMAT = "[{0}]";

        [Header("Channel Configuration")]
        [SerializeField]
        [Range(1, 10)]
        private int m_DefaultChannelCount = 1;

        public UnityTypedEvent.OrderedDictionaryEvent onChannelsUpdate = new UnityTypedEvent.OrderedDictionaryEvent();

        [Header("Messager Configuration")]
        [SerializeField]
        private NetLogger m_NetLogger = null;
        [SerializeField]
        private Dropdown m_ChannelDropdown = null;

        public UnityEvent onCurrentChannelIndexReset = new UnityEvent();

        private GameLogicBase m_GameLogic = null;

        private Dictionary<int, ChannelModel> m_ActiveChannels = new Dictionary<int, ChannelModel>();
        private HashSet<int> m_EmptyChannelIds = new HashSet<int>();
        //private Dictionary<int, List<int>> m_ActivePlayerNumbers = new Dictionary<int, List<int>>();

        private List<ChannelModel> m_AvailableChannels = new List<ChannelModel>();
        private List<int> m_AvailablePlayerNumbers = new List<int>();
        private int m_CurrentChannelIndex = -1;
        private string m_Message = string.Empty;

        void Awake()
        {
            m_GameLogic = FindObjectOfType<GameLogicBase>();
        }

        void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                InitMaster();
            }
            else
            {
                InitOtherPlayer();
            }
        }

        #region Master
        private void InitMaster()
        {
            m_ActiveChannels = new Dictionary<int, ChannelModel>();
            m_AvailableChannels = new List<ChannelModel>();
            for (int i = 0; i < MAX_CHANNEL_COUNT; i++)
            {
                if (i < m_DefaultChannelCount)
                {
                    ChannelModel channel = new ChannelModel(i);
                    m_ActiveChannels.Add(i, channel);
                    m_AvailableChannels.Add(channel);
                }
                else
                {
                    m_EmptyChannelIds.Add(i);
                }
            }

            //默认0号频道为全部玩家频道
            m_ActiveChannels[0].ChannelName = ALL_CHANNEL_NAME;

            if (m_GameLogic.IsInitialized)
            {
                m_AvailablePlayerNumbers = Enumerable.Range(1, m_GameLogic.PlayerCount - 1).ToList();
            }
            else
            {
                GameLogicBase.GameLogicCallback handler = null;
                handler = () => {
                    m_AvailablePlayerNumbers = Enumerable.Range(1, m_GameLogic.PlayerCount - 1).ToList();
                    m_GameLogic.InitGameLogic_Callback -= handler;
                };
                m_GameLogic.InitGameLogic_Callback += handler;
            }

            UpdateChannelDropdown();
        }

        public void ResetChannels()
        {
            if (!PhotonNetwork.isMasterClient) { return; }
            
            foreach (int id in m_ActiveChannels.Keys)
            {
                m_ActiveChannels[id].PlayerNumbers = new HashSet<int>();
            }            
            ReleaseAllPlayersFromChannels();
            
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

            UpdateChannelDropdown();
        }

        private void ReleaseAllPlayersFromChannels()
        {
            photonView.RPC("UnsubscribeFromChannels", PhotonTargets.Others);
        }

        private void ReleasePlayersFromChannel(int channelId)
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;

            foreach (int playerNumber in m_ActiveChannels[channelId].PlayerNumbers)
            {
                photonView.RPC("UnsubscribeFromChannel", playerIdentities[playerNumber].Player, channelId);
            }
        }

        public void UpdateChannel(int channelId, string newName)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;
            m_ActiveChannels[channelId].ChannelName = newName;
            foreach (int playerNumber in m_ActiveChannels[channelId].PlayerNumbers)
            {
                photonView.RPC("SubscribeToOrUpdateChannel", playerIdentities[playerNumber].Player, channelId, m_ActiveChannels[channelId].ChannelName, m_ActiveChannels[channelId].PlayerNumbers.ToArray());
            }

            UpdateChannels();
        }

        public void UpdateChannel(int channelId, int[] newNumbers)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;

            //清理已从频道中移除的玩家
            HashSet<int> removedPlayers = new HashSet<int>(m_ActiveChannels[channelId].PlayerNumbers);
            removedPlayers.ExceptWith(newNumbers);
            foreach (int removedNumber in removedPlayers)
            {
                photonView.RPC("UnsubscribeFromChannel", playerIdentities[removedNumber].Player, channelId);
            }

            //添加新加入的玩家至频道并通知频道中已存在的玩家，更新ActiveChannels
            HashSet<int> addedPlayers = new HashSet<int>(newNumbers);
            m_ActiveChannels[channelId].PlayerNumbers = addedPlayers;
            foreach (int addedNumber in addedPlayers)
            {
                photonView.RPC("SubscribeToOrUpdateChannel", playerIdentities[addedNumber].Player, channelId, m_ActiveChannels[channelId].ChannelName, m_ActiveChannels[channelId].PlayerNumbers.ToArray());
            }

            UpdateChannels();
        }

        public void AddChannel()
        {
            if (!PhotonNetwork.isMasterClient) { return; }
            if (m_EmptyChannelIds.Count == 0) { return; }

            int id = m_EmptyChannelIds.First();
            if (m_EmptyChannelIds.Remove(id))
            {
                ChannelModel channel = new ChannelModel(id);
                m_ActiveChannels.Add(id, channel);
                m_AvailableChannels.Add(channel);

                UpdateChannels();
            }
        }

        public void DeleteChannel(int channelId)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            //清除频道并回收ID
            ReleasePlayersFromChannel(channelId);
            m_ActiveChannels.Remove(channelId);
            m_AvailableChannels.RemoveAll(c => c.ChannelId == channelId);
            m_EmptyChannelIds.Add(channelId);

            UpdateChannels();
        }

        [PunRPC]
        private void RequestToBroadcastMessageInChannel(PhotonPlayer sender, string message, int channelId)
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;

            int senderNumber = playerIdentities.FirstOrDefault(pi => pi.Value.Player.Equals(sender)).Value.Number;
            if (senderNumber < 0) { return; }

            if (!m_ActiveChannels.ContainsKey(channelId)) { return; }
            ChannelModel channel = m_ActiveChannels[channelId];

            if (!sender.isMasterClient)
            {
                //检查玩家广播目标频道的合法性
                if (!channel.PlayerNumbers.Contains(senderNumber)) {
                    photonView.RPC("ReceiveRequestRejection", sender, "不在频道中");
                    return;
                }
            }

            foreach (int receiverNumber in channel.PlayerNumbers)
            {
                if (!playerIdentities.ContainsKey(receiverNumber)) { continue; }
                PhotonPlayer receiver = playerIdentities[receiverNumber].Player;
                if (receiver == null) { continue; }

                if (receiverNumber != senderNumber)
                {
                    photonView.RPC("ReceiveMessageFromChannel", receiver, channelId, senderNumber, message);
                }
            }

            //Master显示消息
            ShowMessage("{0}中的{1}:{2}", channel.ChannelName, sender, message);
        }

        [PunRPC]
        private void RequestToSendMessageToPlayer(PhotonPlayer sender, string message, int receiverNumber)
        {
            Dictionary<int, PlayerIdentity> playerIdentities = m_GameLogic.PlayerIdentities;

            int senderNumber = playerIdentities.FirstOrDefault(pi => pi.Value.Player.Equals(sender)).Value.Number;
            if (senderNumber < 0) { return; }

            if (!playerIdentities.ContainsKey(receiverNumber)) { return; }
            PhotonPlayer receiver = playerIdentities[receiverNumber].Player;
            if (receiver == null) { return; }

            if (!sender.isMasterClient)
            {
                //检查玩家通信目标玩家的合法性
                //todo 
                if (receiverNumber != 0) {
                    photonView.RPC("ReceiveRequestRejection", sender, "无效的接受者");
                    return;
                }
            }

            photonView.RPC("ReceiveMessageFromPlayer", receiver, senderNumber, message);

            //Master显示消息
            ShowMessage("{0}对{1}:{2}", senderNumber, receiverNumber, message);            
        }
        #endregion

        #region All
        private void UpdateChannelDropdown()
        {
            List<Dropdown.OptionData> availableOptions = new List<Dropdown.OptionData>();
            availableOptions.Add(new Dropdown.OptionData("空"));
            foreach (ChannelModel channel in m_AvailableChannels)
            {
                availableOptions.Add(new Dropdown.OptionData(string.Format(CHANNEL_DISPLAY_FORMAT, channel.ChannelName)));
            }
            foreach (int playerNumber in m_AvailablePlayerNumbers)
            {
                availableOptions.Add(new Dropdown.OptionData(string.Format(PLAYER_NUMBER_DISPLAY_FORMAT, playerNumber)));
            }
            m_ChannelDropdown.options = availableOptions;
            m_ChannelDropdown.value = 0;

            m_CurrentChannelIndex = -1;
            onCurrentChannelIndexReset.Invoke();
        }

        public void SetCurrentChannelIndex(int optionIndex)
        {
            //选项0为空
            m_CurrentChannelIndex = optionIndex - 1;
            if (m_CurrentChannelIndex < 0)
            {
                onCurrentChannelIndexReset.Invoke();
            }
        }

        public void SetMessage(string msg)
        {
            m_Message = msg;
        }

        public void SendMessage()
        {
            if (m_CurrentChannelIndex < 0 || m_CurrentChannelIndex >= m_AvailableChannels.Count + m_AvailablePlayerNumbers.Count) { return; }
            
            if (m_CurrentChannelIndex < m_AvailableChannels.Count)
            {
                photonView.RPC("RequestToBroadcastMessageInChannel", PhotonTargets.MasterClient, PhotonNetwork.player, m_AvailableChannels[m_CurrentChannelIndex].ChannelId, m_Message);
            }
            else
            {
                photonView.RPC("RequestToSendMessageToPlayer", PhotonTargets.MasterClient, PhotonNetwork.player, m_AvailablePlayerNumbers[m_CurrentChannelIndex - m_AvailableChannels.Count], m_Message);
            }
        }

        [PunRPC]
        private void ReceiveMessageFromChannel(int channelId, int senderNumber, string message)
        {
            //todo
        }

        [PunRPC]
        private void ReceiveMessageFromPlayer(int senderNumber, string message)
        {
            //todo
        }

        [PunRPC]
        private void ReceiveRequestRejection(string err)
        {
            ShowMessage("消息发送请求失败:{0}", err);
        }

        private void ShowMessage(string format, params object[] args)
        {
            m_NetLogger.AddMessage(string.Format(format, args));
        }
        #endregion

        #region OtherPlayers
        private void InitOtherPlayer()
        {
            m_AvailableChannels = new List<ChannelModel>();

            //默认0号频道为全部玩家频道
            ChannelModel channel = new ChannelModel(0);
            channel.ChannelName = ALL_CHANNEL_NAME;
            m_AvailableChannels.Insert(0, channel);

            m_AvailablePlayerNumbers = new List<int>{0};

            UpdateChannelDropdown();
        }

        [PunRPC]
        private void UnsubscribeFromChannels()
        {
            m_AvailableChannels.RemoveAll(c => c.ChannelId != 0);
            UpdateChannelDropdown();
        }

        [PunRPC]
        private void UnsubscribeFromChannel(int channelId)
        {
            m_AvailableChannels.Remove(m_AvailableChannels.Find(c => c.ChannelId == channelId));
            UpdateChannelDropdown();
        }

        [PunRPC]
        private void SubscribeToOrUpdateChannel(int channelId, string channelName, int[] playerNumbers)
        {
            ChannelModel channel = m_AvailableChannels.Find(c => c.ChannelId == channelId);
            if (channel.Equals(default(ChannelModel)))
            {
                channel = new ChannelModel(channelId);
            }
            channel.ChannelName = channelName;
            channel.PlayerNumbers = new HashSet<int>(playerNumbers);
            m_AvailableChannels.Add(channel);
            UpdateChannelDropdown();
        }        
        #endregion
    }
}
