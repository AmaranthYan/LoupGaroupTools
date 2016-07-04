namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

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
            playerNumbers.Shuffle();

            //分配玩家编号
            for (int i = 0, k = 0; i < m_Players.Length;i++)
            {
                m_PlayerIdentities.Add(m_Players[i].isMasterClient ? 0 : playerNumbers[k++], m_Players[i]);                
            }
            Debug.Log(m_PlayerIdentities.ToStringFull());

            DistributePlayerIdentities();
        }

        public override void DistributePlayerIdentities()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            foreach (KeyValuePair<int, PhotonPlayer> playerIdentity in m_PlayerIdentities)
            {
                photonView.RPC("ReceivePlayerIdentity", playerIdentity.Value, playerIdentity.Value, playerIdentity.Key);
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
