namespace LoupGarou
{
    using Photon;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;

    public class NetPlayer : PunBehaviour
    {
        public const float SERVER_REFRESH_RATE = 0.25f;

        [SerializeField]
        private PhotonIdentifier m_PhotonIdentifier = null;
        private IEnumerator m_RequestRoomList_Coroutine = null;

        [Header("Events")]
        public UnityTypedEvent.StringEvent onPhotonEvent = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.PhotonRoomInfoArrayEvent onRoomListUpdate = new UnityTypedEvent.PhotonRoomInfoArrayEvent();
        public UnityEvent onJoinRoom = new UnityEvent();
        public UnityEvent onLeaveRoom = new UnityEvent();

        void Awake()
        {
            if (!m_PhotonIdentifier) {
                Debug.LogError("PUN项目配置错误！");
                this.enabled = false;
            };
        }
        
        void Start()
        {
            ConnectToPUN();
        }

        void Update()
        {

        }

        #region PhotonCallbacks
        public override void OnConnectedToPhoton()
        {
            base.OnConnectedToPhoton();
            onPhotonEvent.Invoke("已连接至PUN。");
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            onPhotonEvent.Invoke("已接入Photon服务器。");
        }

        public override void OnReceivedRoomListUpdate()
        {
            base.OnReceivedRoomListUpdate();
            FetchRoomList();
            onPhotonEvent.Invoke("房间列表已更新。");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            onJoinRoom.Invoke();
            onPhotonEvent.Invoke("已加入房间\"" + PhotonNetwork.room.name + "\"，当前人数为" + PhotonNetwork.room.playerCount + "人。");
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            onLeaveRoom.Invoke();
            onPhotonEvent.Invoke("已离开房间。");
        }        
        #endregion

        public void FetchRoomList()
        {
            RoomInfo[] roomInfo = PhotonNetwork.GetRoomList();
            onRoomListUpdate.Invoke(roomInfo);
        }

        private void ConnectToPUN()
        {
            onPhotonEvent.Invoke("正在连接PUN...");
            PhotonNetwork.ConnectUsingSettings(m_PhotonIdentifier.GameIdentifier + '_' + m_PhotonIdentifier.GameVersion);
        }
    }
}
