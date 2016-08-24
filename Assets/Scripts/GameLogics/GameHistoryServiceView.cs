namespace LoupsGarous
{
    using Photon;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class GameHistoryServiceView : PunBehaviour
    {
        public UnityEvent onGameHistoryAlert = new UnityEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onPlayerIdentitiesHistoryUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onUnusedIdentitiesHistoryUpdate = new UnityTypedEvent.OrderedDictionaryEvent();

        private Dictionary<int, Dictionary<int, PlayerIdentity>> m_PlayerIdentitiesHistory = new Dictionary<int, Dictionary<int, PlayerIdentity>>();
        private Dictionary<int, List<PlayerIdentity>> m_UnusedIdentitiesHistory = new Dictionary<int, List<PlayerIdentity>>();

        private int m_GameHistoryIndex = -1;

        #region Master
        public void RecordIdentitiesHistory(Dictionary<int, PlayerIdentity> playerIdentities, List<PlayerIdentity> unusedIdentities)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            m_GameHistoryIndex++;

            m_PlayerIdentitiesHistory[m_GameHistoryIndex] = playerIdentities;
            m_UnusedIdentitiesHistory[m_GameHistoryIndex] = unusedIdentities;
        }

        public void BroadcastGameHistory()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            photonView.RPC("AlertGameHistory", PhotonTargets.Others);
            BroadcastIdentitiesHistory();

            //更新本地历史
            onGameHistoryAlert.Invoke();
            UpdateIdentitiesHistory();
        }

        private void BroadcastIdentitiesHistory()
        {
            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_PlayerIdentitiesHistory[m_GameHistoryIndex])
            {
                photonView.RPC("ReceivePlayerIdentityHistory", PhotonTargets.Others, playerIdentity.Key, playerIdentity.Value.Player, playerIdentity.Value.CharacterId);
            }
            foreach (PlayerIdentity unusedIdentity in m_UnusedIdentitiesHistory[m_GameHistoryIndex])
            {
                photonView.RPC("ReceiveUnusedIdentityHistory", PhotonTargets.Others, unusedIdentity.Number, unusedIdentity.CharacterId);
            }

            photonView.RPC("UpdateIdentitiesHistory", PhotonTargets.Others);
        }
        #endregion

        #region OtherPlayers
        [PunRPC]
        private void AlertGameHistory()
        {
            m_GameHistoryIndex++;

            m_PlayerIdentitiesHistory[m_GameHistoryIndex] = new Dictionary<int, PlayerIdentity>();
            m_UnusedIdentitiesHistory[m_GameHistoryIndex] = new List<PlayerIdentity>();

            onGameHistoryAlert.Invoke();
        }

        [PunRPC]
        private void ReceivePlayerIdentityHistory(int playerNumber, PhotonPlayer player, int characterId)
        {
            PlayerIdentity playerIdentity = new PlayerIdentity();
            playerIdentity.UpdateNumber(playerNumber);
            playerIdentity.UpdatePlayer(player);
            playerIdentity.UpdateIdentity(characterId);

            m_PlayerIdentitiesHistory[m_GameHistoryIndex][playerNumber] = playerIdentity;
        }

        [PunRPC]
        private void ReceiveUnusedIdentityHistory(int index, int characterId)
        {
            PlayerIdentity unusedIdentity = new PlayerIdentity();
            unusedIdentity.UpdateNumber(index);
            unusedIdentity.UpdateIdentity(characterId);

            m_UnusedIdentitiesHistory[m_GameHistoryIndex].Add(unusedIdentity);
        }

        [PunRPC]
        private void UpdateIdentitiesHistory()
        {
            UpdatePlayerIdentitiesHistory();
            UpdateUnusedIdentitiesHistory();
        }

        private void UpdatePlayerIdentitiesHistory()
        {
            List<int> playerNumbers = m_PlayerIdentitiesHistory[m_GameHistoryIndex].Keys.ToList();
            playerNumbers.Sort();

            OrderedDictionary dictionary = new OrderedDictionary();
            for (int i = 0; i < playerNumbers.Count; i++)
            {
                if (playerNumbers[i] != 0)
                {
                    dictionary.Add(playerNumbers[i].ToString(), m_PlayerIdentitiesHistory[m_GameHistoryIndex][i]);
                }
            }

            onPlayerIdentitiesHistoryUpdate.Invoke(dictionary);
        }

        private void UpdateUnusedIdentitiesHistory()
        {
            OrderedDictionary dictionary = new OrderedDictionary();
            for (int i = 0; i < m_UnusedIdentitiesHistory[m_GameHistoryIndex].Count; i++)
            {
                dictionary.Add(i.ToString(), m_UnusedIdentitiesHistory[m_GameHistoryIndex][i]);
            }

            onUnusedIdentitiesHistoryUpdate.Invoke(dictionary);
        }
        #endregion
    }
}