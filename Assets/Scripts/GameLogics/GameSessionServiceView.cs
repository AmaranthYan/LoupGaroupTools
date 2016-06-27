namespace LoupsGarous
{
    using Photon;
    using UnityEngine;

    [RequireComponent(typeof(PhotonView))]
    public class GameSessionServiceView : PunBehaviour
    {
        [SerializeField]
        private GameConfigView m_GameConfigView = null;

        [PunRPC]
        public void StartGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                photonView.RPC("StartGameSession", PhotonTargets.Others);
            }
            GameSessionService.StartGameSession(m_GameConfigView);
            GameSessionService.GameSession.transform.parent = this.transform;
        }
    }
}
