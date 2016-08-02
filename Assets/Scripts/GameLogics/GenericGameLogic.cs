namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GenericGameLogic : GameLogicBase
    {
        void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (isInitialized)
                {
                    GeneratePlayerIdentities();
                }
                else
                {
                    GameLogicCallback handler = null;
                    handler = () => {
                        GeneratePlayerIdentities();
                        m_InitGameLogic_Callback -= handler;
                    };
                    m_InitGameLogic_Callback += handler;
                }
            }
        }

        #region PhotonCallbacks
        //若上帝玩家离开则强制结束游戏进程
        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);

            if (otherPlayer.isMasterClient)
            {
                FindObjectOfType<GameSessionServiceView>().EndLocalGameSession();
            }
        }
        #endregion

        public override void GeneratePlayerIdentities()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            //随机生成玩家编号，0号预留给上帝玩家(MasterPlayer)
            List<int> playerNumbers = new List<int>();
            for (int i = 1; i < m_Players.Length; i++)
            {
                playerNumbers.Add(i);
            }
            playerNumbers = playerNumbers.Shuffle().ToList();

            //分配玩家编号
            photonView.RPC("ReceivePlayerNumbers", PhotonTargets.All, playerNumbers.ToArray());

            //生成所有角色
            List<int> characterIds = new List<int>();
            foreach (int id in m_CharacterSet.Keys)
            {
                characterIds.AddRange(Enumerable.Repeat(id, m_CharacterSet[id]));
            }
            characterIds = characterIds.Shuffle().ToList();

            int k = 0;
            foreach (int index in m_PlayerIdentities.Keys)
            {
                PlayerIdentity playerIdentity = m_PlayerIdentities[index];
                playerIdentity.UpdateIdentity(characterIds[k++]);
                playerIdentity.MarkAsRevealed(true);
            }
            UpdatePlayerIdentities();

            for (int i = 0; i < characterIds.Count() - k; i++)
            {
                PlayerIdentity unusedIdentity = m_UnusedIdentity[i];
                unusedIdentity.UpdateIdentity(characterIds[k + i]);
                unusedIdentity.MarkAsRevealed(true);
            }
            UpdateUnusedIdentities();            

            //分配玩家角色
            DistributePlayerIdentities();
        }

        [PunRPC]
        protected override void ReceivePlayerNumbers(int[] playerNumbers)
        {
            if (playerNumbers.Length != m_Players.Length - 1)
            {
                Debug.LogError("玩家编号数量与玩家人数不符！");
            }

            for (int i = 0, k = 0; i < m_Players.Length; i++)
            {
                m_PlayerIdentities[m_Players[i].isMasterClient ? 0 : playerNumbers[k++]].UpdatePlayer(m_Players[i]);           
            }

            UpdatePlayerIdentities();
        }

        public override void DistributePlayerIdentities()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_PlayerIdentities)
            {
                if (playerIdentity.Key != 0)
                {
                    photonView.RPC("ReceivePlayerIdentity", playerIdentity.Value.Player, playerIdentity.Value.Number, playerIdentity.Value.CharacterId);
                }                
            }
        }

        public override void ReversePlayerSurvivalStatus(int number)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            m_PlayerIdentities[number].MarkAsDead(!m_PlayerIdentities[number].IsDead);
            onSinglePlayerIdentityUpdate.Invoke(number.ToString(), m_PlayerIdentities[number]);

            photonView.RPC("MarkPlayerAsDead", PhotonTargets.Others, number, m_PlayerIdentities[number].IsDead);
        }

        [PunRPC]
        protected override void MarkPlayerAsDead(int number, bool isDead)
        {
            m_PlayerIdentities[number].MarkAsDead(isDead);
            onSinglePlayerIdentityUpdate.Invoke(number.ToString(), m_PlayerIdentities[number]);
        }

        public override void PromotePlayerToCaptain(int number)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            if (m_PlayerIdentities.ContainsKey(captainNumber))
            {
                m_PlayerIdentities[captainNumber].MarkAsCaptain(false);
                onSinglePlayerIdentityUpdate.Invoke(captainNumber.ToString(), m_PlayerIdentities[captainNumber]);
            }
            captainNumber = number != captainNumber ? number : -1;
            if (m_PlayerIdentities.ContainsKey(captainNumber))
            {
                m_PlayerIdentities[captainNumber].MarkAsCaptain(true);
                onSinglePlayerIdentityUpdate.Invoke(captainNumber.ToString(), m_PlayerIdentities[captainNumber]);
            }

            photonView.RPC("MarkPlayerAsCaptain", PhotonTargets.Others, captainNumber);
        }

        [PunRPC]
        protected override void MarkPlayerAsCaptain(int number)
        {
            if (m_PlayerIdentities.ContainsKey(captainNumber))
            {
                m_PlayerIdentities[captainNumber].MarkAsCaptain(false);
                onSinglePlayerIdentityUpdate.Invoke(captainNumber.ToString(), m_PlayerIdentities[captainNumber]);
            }

            if (m_PlayerIdentities.ContainsKey(number))
            {
                m_PlayerIdentities[number].MarkAsCaptain(true);
                onSinglePlayerIdentityUpdate.Invoke(number.ToString(), m_PlayerIdentities[number]);
            }
        }

        public override void BroadcastPlayerIdentity(int number)
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            photonView.RPC("ReceivePlayerIdentity", PhotonTargets.Others , number, m_PlayerIdentities[number].CharacterId);
        }

        public override void BroadcastUnusedIdentity(int index)
        {     
            if (!PhotonNetwork.isMasterClient) { return; }

            photonView.RPC("ReceiveUnusedIdentity", PhotonTargets.Others , index, m_UnusedIdentity[index].CharacterId);
        }

        [PunRPC]
        protected override void ReceivePlayerIdentity(int number, int characterId)
        {
            PlayerIdentity playerIdentity = m_PlayerIdentities[number];
            playerIdentity.UpdateIdentity(characterId);
            playerIdentity.MarkAsRevealed(true);
            onSinglePlayerIdentityUpdate.Invoke(number.ToString(), playerIdentity);
        }

        [PunRPC]
        protected override void ReceiveUnusedIdentity(int index, int characterId)
        {
            PlayerIdentity unusedIdentity = m_UnusedIdentity[index];
            unusedIdentity.UpdateIdentity(characterId);
            unusedIdentity.MarkAsRevealed(true);
            onSingleUnusedIdentityUpdate.Invoke(index.ToString(), unusedIdentity);
        }
    }
}
