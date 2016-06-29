namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(PhotonView))]
    public class GameSessionServiceView : PunBehaviour
    {
        [SerializeField]
        private GameConfigView m_GameConfigView = null;

        public UnityEvent onStartRemoteGameSession = new UnityEvent();
        public UnityEvent onEndRemoteGameSession = new UnityEvent();
        public UnityEvent onEndLocalGameSession = new UnityEvent();

        public void StartGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                PhotonNetwork.room.open = false;
                photonView.RPC("StartRemoteGameSession", PhotonTargets.AllViaServer, PhotonNetwork.playerList, m_GameConfigView);
            }
        }

        [PunRPC]
        private void StartRemoteGameSession(PhotonPlayer[] players, GameConfigView gameConfig)
        {
            bool hasStarted = GameSessionService.StartGameSession(players, gameConfig);
            if (PhotonNetwork.player.isMasterClient)
            {
                PhotonNetwork.room.open = !hasStarted;
            }
            GameSessionService.GameSession.transform.parent = this.transform;
            onStartRemoteGameSession.Invoke();
        }

        public void EndGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                photonView.RPC("EndRemoteGameSession", PhotonTargets.AllViaServer);
                PhotonNetwork.room.open = true;
            }
        }

        [PunRPC]
        private void EndRemoteGameSession()
        {
            GameSessionService.EndGameSession();
            onEndRemoteGameSession.Invoke();
        }

        public void EndLocalGameSession()
        {
            GameSessionService.EndGameSession();
            onEndLocalGameSession.Invoke();
        }
    }
}
