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
                m_PlayerIdentities[index].UpdateIdentity(characterIds[k++]);
                //m_PlayerIdentities[m_Players[i].isMasterClient ? 0 : playerNumbers[k++]].UpdatePlayer(m_Players[i]);
            }
            UpdatePlayerIdentities();

            for (int i = k; i < characterIds.Count(); i++)
            {
                //m_UnusedIdentity.Add
            }
            UpdateUnusedIdentities();
            

            //分配玩家角色
            //DistributePlayerCharacters();

            //分配剩余角色
            //DistributeUnusedIdentities();
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

        public override void DistributePlayerCharacters()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            foreach (KeyValuePair<int, PlayerIdentity> playerIdentity in m_PlayerIdentities)
            {
                photonView.RPC("ReceivePlayerIdentity", playerIdentity.Value.Player, playerIdentity.Value.Player, playerIdentity.Value.CharacterId);
            }
        }

        public override void BroadcastPlayerIdentity(PhotonPlayer player)
        {
            if (!PhotonNetwork.isMasterClient) { return; }
        }

        [PunRPC]
        protected override void ReceivePlayerIdentity(PhotonPlayer player, int characterId)
        {
            Debug.Log(player + " : " + characterId);
            if (player.Equals(PhotonNetwork.player))
            {

            }
            else
            {

            }
        }        
    }
}
