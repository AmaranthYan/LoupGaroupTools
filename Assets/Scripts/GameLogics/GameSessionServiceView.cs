namespace LoupsGarous
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Linq;

    [RequireComponent(typeof(PhotonView))]
    public class GameSessionServiceView : PunBehaviour
    {
        [SerializeField]
        private GameModeDatabase m_GameModeDatabase = null;
        [SerializeField]
        private GameConfigView m_GameConfigView = null;
        
        public UnityTypedEvent.StringEvent onLogUpdate = new UnityTypedEvent.StringEvent();

        public UnityEvent onStartRemoteGameSession = new UnityEvent();
        public UnityEvent onEndRemoteGameSession = new UnityEvent();
        public UnityEvent onEndLocalGameSession = new UnityEvent();

        public void StartGameSession()
        {
            if (PhotonNetwork.player.isMasterClient)
            {
                PhotonNetwork.room.open = false;
                int gameModeId;
                Dictionary<int, int> characterSet;
                int maxPlayerNumber = GameSessionService.SerializeGameConfigParameters(m_GameConfigView, out gameModeId, out characterSet);
                if (PhotonNetwork.playerList.Length <= maxPlayerNumber)
                {
                    int[] allocatedPhotonViewIds = GameSessionService.AllocatePhotonViewIds();
                    photonView.RPC("StartRemoteGameSession", PhotonTargets.AllViaServer, allocatedPhotonViewIds, PhotonNetwork.playerList, gameModeId, characterSet);
                }
                else
                {
                    Debug.LogError("角色数量不足，游戏未能成功创建！");
                }
            }
        }

        [PunRPC]
        private void StartRemoteGameSession(int[] allocatedPhotonViewIds, PhotonPlayer[] players, int gameModeId, Dictionary<int, int> characterSet)
        {
            GameModeModel gameMode = null;
            foreach (GameModeModel gameModeModel in m_GameModeDatabase.GameModeModels)
            {
                if (gameModeModel.Id == gameModeId)
                {
                    gameMode = gameModeModel;
                    break;
                }
            }
            int[] otherPhotonViewIds = allocatedPhotonViewIds.Skip(1).ToArray();
            bool hasStarted = GameSessionService.StartGameSession(allocatedPhotonViewIds[0], otherPhotonViewIds, players, gameMode, characterSet);
            if (PhotonNetwork.player.isMasterClient)
            {
                PhotonNetwork.room.open = !hasStarted;
            }
            GameSessionService.GameSession.transform.parent = this.transform;
            onStartRemoteGameSession.Invoke();
            UpdateLog(Timestamp.ImprintLocalTime() + "游戏已开始。");
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
            UpdateLog(Timestamp.ImprintLocalTime() + "当前游戏已结束。");
        }

        public void EndLocalGameSession()
        {
            GameSessionService.EndGameSession();
            onEndLocalGameSession.Invoke();
            UpdateLog(Timestamp.ImprintLocalTime() + "已退出当前游戏。");
        }

        public void UpdateLog(string log)
        {
            onLogUpdate.Invoke(log);
        }
    }
}
