namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class MessageChannelView : ScrollListItemView<MessageChannelModel>
    {
        private const string CHANNEL_NAME_REGEX = @"^\{([\p{L}0-9]+)\}$";
        private const string PLAYER_NUMBERS_REGEX = @"\[([0-9]+)\]";

        [Header("Events")]
        public UnityTypedEvent.BoolEvent onIsDefaultChannel = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.StringEvent onChannelNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.StringEvent onPlayerNumbersUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.BoolEvent onIsMessageChannelAsync = new UnityTypedEvent.BoolEvent();

        MessageChannelModel m_AsyncItem = new MessageChannelModel();

        void Start()
        {
            onIsMaster.Invoke(PhotonNetwork.isMasterClient);
        }

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Item = (MessageChannelModel)value;
            m_AsyncItem = new MessageChannelModel(m_Item.ChannelId);
            m_AsyncItem.ChannelName = m_Item.ChannelName;
            m_AsyncItem.PlayerNumbers = m_Item.PlayerNumbers;
            onIsMessageChannelAsync.Invoke(IsAsync());

            onIsDefaultChannel.Invoke(m_Item.ChannelId == 0);
            DisplayChannelName(m_Item.ChannelName);
            DisplayPlayerNumbers(m_Item.PlayerNumbers);
        }

        private void DisplayChannelName(string channelName)
        {
            onChannelNameUpdate.Invoke(string.IsNullOrEmpty(channelName) ? string.Empty : string.Format(Messager.CHANNEL_DISPLAY_FORMAT, channelName));
        }

        private void DisplayPlayerNumbers(HashSet<int> playerNumbers)
        {
            List<int> numbers = playerNumbers.ToList();
            numbers.Sort();

            string display = string.Empty;
            foreach (int number in numbers)
            {
                display += string.Format(Messager.PLAYER_NUMBER_DISPLAY_FORMAT, number);
            }
            onPlayerNumbersUpdate.Invoke(display);
        }

        private bool IsAsync()
        {
            return IsChannelNameAsync() || IsPlayerNumbersAsync();
        }

        private bool IsChannelNameAsync()
        {
            return !m_AsyncItem.ChannelName.Equals(m_Item.ChannelName);
        }

        private bool IsPlayerNumbersAsync()
        {
            return !m_AsyncItem.PlayerNumbers.SetEquals(m_Item.PlayerNumbers);
        }

        public void UpdateLocalChannelName(string nameText)
        {
            Regex regex = new Regex(CHANNEL_NAME_REGEX);
            Match match = regex.Match(nameText);
            string name = match.Success ? match.Groups[1].Value : string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                m_AsyncItem.ChannelName = name;
                onIsMessageChannelAsync.Invoke(IsAsync());
            }
            DisplayChannelName(m_AsyncItem.ChannelName);
        }

        public void UpdateLocalPlayerNumbers(string numbersText)
        {
            Regex regex = new Regex(PLAYER_NUMBERS_REGEX);
            Match match = regex.Match(numbersText);
            HashSet<int> playerNumbers = new HashSet<int>();
            Messager messager = (MessageChannelListView)m_ScrollListView.Messager;
            while (match.Success)
            {
                int number;
                if (int.TryParse(match.Groups[1].Value, out number))
                {
                    if (messager.ValidatePlayerNumber(number))
                    {
                        playerNumbers.Add(number);
                    }
                }                
                match = match.NextMatch();
            }
            m_AsyncItem.PlayerNumbers = playerNumbers;
            DisplayPlayerNumbers(m_AsyncItem.PlayerNumbers);

            onIsMessageChannelAsync.Invoke(IsAsync());
        }

        public void SyncMessageChannel()
        {
            Messager messager = (MessageChannelListView)m_ScrollListView.Messager;
            if (!messager) { return; }
            
            if (IsPlayerNumbersAsync())
            {
                messager.UpdateChannel(m_AsyncItem.ChannelId, m_AsyncItem.PlayerNumbers.ToArray());
            }

            if (IsChannelNameAsync())
            {
                messager.UpdateChannel(m_AsyncItem.ChannelId, m_AsyncItem.ChannelName);
            }
        }
    }
}