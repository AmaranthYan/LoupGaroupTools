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
        //若上帝玩家(发生MasterPlayer切换)离开则强制结束游戏进程
        public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);

            if (PhotonNetwork.player.Equals(newMasterClient))
            {
                FindObjectOfType<GameSessionServiceView>().EndGameSession();
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
            for (int i = 0; i < m_Players.Length;i++)
            {
                m_PlayerIdentities.Add(m_Players[i].isMasterClient ? 0 : playerNumbers[i], m_Players[i]);
                Debug.Log((m_Players[i].isMasterClient ? 0 : playerNumbers[i]) + " : " + m_Players[i]);
            }

            DistributePlayerIdentities();
        }

        public override void DistributePlayerIdentities()
        {
            if (!PhotonNetwork.isMasterClient) { return; }

            photonView.RPC("ReceivePlayerIdentity", m_Players[0]);
        }

        public override void BroadcastPlayerIdentity(PhotonPlayer player)
        {
            if (!PhotonNetwork.isMasterClient) { return; }
        }

        [PunRPC]
        protected override void ReceivePlayerIdentity(PhotonPlayer player, int characterId)
        {
            if (player.Equals(PhotonNetwork.player))
            {

            }
            else
            {

            }
        }        
    }
}
