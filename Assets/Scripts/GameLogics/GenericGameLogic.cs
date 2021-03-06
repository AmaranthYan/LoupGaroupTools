﻿namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GenericGameLogic : GameLogicBase
    {
        public UnityEvent onPlayerDisconnect = new UnityEvent();

        public UnityEvent onIsMaster = new UnityEvent();
        public UnityEvent onIsNotMaster = new UnityEvent();
        public UnityEvent onGameRestart = new UnityEvent();

        void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (m_IsInitialized)
                {
                    GeneratePlayerIdentities();
                }
                else
                {
                    GameLogicCallback handler = null;
                    handler = () => {
                        GeneratePlayerIdentities();
                        InitGameLogic_Callback -= handler;
                    };
                    InitGameLogic_Callback += handler;
                }

                onIsMaster.Invoke();
            }
            else
            {
                onIsNotMaster.Invoke();
            }
        }

        #region PhotonCallbacks
        //若上帝玩家离开(或因未知原因发生Master转移)则强制结束游戏进程
        public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            FindObjectOfType<GameSessionServiceView>().UpdateLog("<color=#800000ff>" + Timestamp.ImprintLocalTime() + "房主已丢失，即将结束当前游戏...</color>");
            EndLocalGame();
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);

            onPlayerDisconnect.Invoke();
            /**
            if (otherPlayer.isMasterClient)
            {
                EndLocalGame();
            }
            */
        }
        #endregion
        
        public override void RestartGame()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            FindObjectOfType<GameHistoryServiceView>().BroadcastGameHistory();

            photonView.RPC("GenerateEmptyIdenities", PhotonTargets.Others);
            GenerateEmptyIdenities();
            GeneratePlayerIdentities();

            onGameRestart.Invoke();
        }

        public override void EndGame()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            FindObjectOfType<GameHistoryServiceView>().BroadcastGameHistory();
            FindObjectOfType<GameSessionServiceView>().EndGameSession();
        }

        public override void EndLocalGame()
        {
            FindObjectOfType<GameSessionServiceView>().EndLocalGameSession();
        }

        public override void GeneratePlayerIdentities()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            //随机生成玩家编号，0号预留给上帝玩家
            List<int> playerNumbers = new List<int>();
            for (int i = 1; i < m_Players.Length; i++)
            {
                playerNumbers.Add(i);
            }
            playerNumbers = playerNumbers.Shuffle().ToList();

            //向其他玩家分配编号列表
            photonView.RPC("ReceivePlayerNumbers", PhotonTargets.All, playerNumbers.ToArray());

            //生成所有游戏身份
            List<int> characterIds = new List<int>();
            foreach (int id in m_CharacterSet.Keys)
            {
                characterIds.AddRange(Enumerable.Repeat(id, m_CharacterSet[id]));
            }
            characterIds = characterIds.Shuffle().ToList();

            //对应编号分配游戏身份，上帝(0号玩家)本身不获得游戏身份
            int k = 0;
            foreach (int index in m_PlayerIdentities.Keys)
            {
                if (index != 0)
                {
                    PlayerIdentity playerIdentity = m_PlayerIdentities[index];
                    playerIdentity.UpdateIdentity(characterIds[k++]);
                    playerIdentity.MarkAsRevealed(true);
                }
            }
            UpdatePlayerIdentities();

            //收集多余游戏身份
            for (int i = 0; i < characterIds.Count() - k; i++)
            {
                PlayerIdentity unusedIdentity = m_UnusedIdentities[i];
                unusedIdentity.UpdateIdentity(characterIds[k + i]);
                unusedIdentity.MarkAsRevealed(true);
            }
            UpdateUnusedIdentities();

            FindObjectOfType<GameHistoryServiceView>().RecordIdentitiesHistory(m_PlayerIdentities, m_UnusedIdentities);

            //向其他玩家分配各自身份
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

            if (m_PlayerIdentities.ContainsKey(m_CaptainNumber))
            {
                m_PlayerIdentities[m_CaptainNumber].MarkAsCaptain(false);
                onSinglePlayerIdentityUpdate.Invoke(m_CaptainNumber.ToString(), m_PlayerIdentities[m_CaptainNumber]);
            }
            m_CaptainNumber = number != m_CaptainNumber ? number : -1;
            if (m_PlayerIdentities.ContainsKey(m_CaptainNumber))
            {
                m_PlayerIdentities[m_CaptainNumber].MarkAsCaptain(true);
                onSinglePlayerIdentityUpdate.Invoke(m_CaptainNumber.ToString(), m_PlayerIdentities[m_CaptainNumber]);
            }

            photonView.RPC("MarkPlayerAsCaptain", PhotonTargets.Others, m_CaptainNumber);
        }

        [PunRPC]
        protected override void MarkPlayerAsCaptain(int number)
        {
            if (m_PlayerIdentities.ContainsKey(m_CaptainNumber))
            {
                m_PlayerIdentities[m_CaptainNumber].MarkAsCaptain(false);
                onSinglePlayerIdentityUpdate.Invoke(m_CaptainNumber.ToString(), m_PlayerIdentities[m_CaptainNumber]);
            }
            m_CaptainNumber = number;
            if (m_PlayerIdentities.ContainsKey(m_CaptainNumber))
            {
                m_PlayerIdentities[m_CaptainNumber].MarkAsCaptain(true);
                onSinglePlayerIdentityUpdate.Invoke(m_CaptainNumber.ToString(), m_PlayerIdentities[m_CaptainNumber]);
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

            photonView.RPC("ReceiveUnusedIdentity", PhotonTargets.Others , index, m_UnusedIdentities[index].CharacterId);
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
            PlayerIdentity unusedIdentity = m_UnusedIdentities[index];
            unusedIdentity.UpdateIdentity(characterId);
            unusedIdentity.MarkAsRevealed(true);
            onSingleUnusedIdentityUpdate.Invoke(index.ToString(), unusedIdentity);
        }
    }
}
