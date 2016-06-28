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
                GeneratePlayerIdentities();
            }
        }

        private void GeneratePlayerIdentities()
        {
            //列表0号预留给MasterClient
            DistributePlayerIdentities();
        }

        public void DistributePlayerIdentities()
        {
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("ReceivePlayerCharacter", m_Players[0]);
            }
        }

        public void BroadcastPlayerIdentity(PhotonPlayer player)
        {
            if (PhotonNetwork.isMasterClient)
            {
            }
        }

        [PunRPC]
        private void ReceivePlayerIdentity(PhotonPlayer player, int characterId)
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
