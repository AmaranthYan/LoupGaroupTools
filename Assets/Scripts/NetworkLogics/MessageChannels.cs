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
            public string ChannelName;
            public int[] PlayerNumbers;

            public ChannelModel()
            {
                ChannelName = string.Empty;
                PlayerNumbers = new int[0];
            }
        }

        private Dictionary<int, ChannelModel> m_AllChannels = new Dictionary<int, ChannelModel>();
        private Dictionary<int, List<string>> m_PlayerAvailableChannelList = new Dictionary<int, List<string>>();

        private List<ChannelModel> m_AvailableChannels = new List<ChannelModel>();
        private string m_CurrentChannelName = string.Empty;
        private string m_Message = string.Empty;

        #region Master
        public void InitChannels()
        {
            m_AllChannels.Clear();
            //todo
            UpdateChannels();
        }

        private void UpdateChannels()
        {
            OrderedDictionary dictionary = new OrderedDictionary();
        }

        public void AddChannel()
        {

        }
        #endregion

        #region OtherPlayers
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
