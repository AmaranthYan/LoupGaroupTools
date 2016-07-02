using Photon;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Specialized;

public class NetPlayer : PunBehaviour
{
    public const string ANONYMOUS = "匿名爆狼";
    public const string ANONYMOUS_FORMAT = "{0}#{1:00}";

    public const float SECONDS_BETWEEN_CONNECT_ATTEMPTS = 2;

    [SerializeField]
    private PhotonIdentifier m_PhotonIdentifier = null;

    private bool m_IsConnectionInitialized = false;
    private IEnumerator m_ReconnectToPUN_Coroutine = null;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPhotonEvent = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.OrderedDictionaryEvent onRoomListUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
    public UnityTypedEvent.OrderedDictionaryEvent onPlayerListUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
    public UnityEvent onJoinRoom = new UnityEvent();
    public UnityEvent onLeaveRoom = new UnityEvent();
    public UnityEvent onBecomeMasterPlayer = new UnityEvent();
    public UnityEvent onNotBecomeMasterPlayer = new UnityEvent();

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
        m_IsConnectionInitialized = true;
        onPhotonEvent.Invoke(ImprintLocalTime() + "已连接至PUN。");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);
        Debug.LogError("连接PUN失败，原因[" + cause + "]");
        onPhotonEvent.Invoke("<color=#800000ff>" + ImprintLocalTime() + "连接PUN失败！</color>");
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已从PUN断开。");

        if (!m_IsConnectionInitialized) { return; }

        if (m_ReconnectToPUN_Coroutine != null)
        {
            StopCoroutine(m_ReconnectToPUN_Coroutine);
        }
        m_ReconnectToPUN_Coroutine = ReconnectToPUN();
        StartCoroutine(m_ReconnectToPUN_Coroutine);
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        base.OnConnectionFail(cause);
        onPhotonEvent.Invoke("<color=#800000ff>" + ImprintLocalTime() + "PUN连接中断！</color>");
    }

    public override void OnPhotonMaxCccuReached()
    {
        base.OnPhotonMaxCccuReached();
        onPhotonEvent.Invoke("<color=#800000ff>" + ImprintLocalTime() + "当前服务器连接已达到上限！</color>");
    }

    //因为使用DefaultLobby所以OnConnectedToMaster()不会被调用
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已接入大厅。");
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已离开大厅。");
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        FetchRoomList();
        onPhotonEvent.Invoke(ImprintLocalTime() + "房间列表已更新。");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已创建房间\"" + PhotonNetwork.room.name + "\"。");
        PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        onBecomeMasterPlayer.Invoke();
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonCreateRoomFailed(codeAndMsg);
        //short code = (short)codeAndMsg[0];
        //string msg = (string)codeAndMsg[1];
        onPhotonEvent.Invoke("<color=#800000ff>" + ImprintLocalTime() + "创建房间失败！</color>");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        onJoinRoom.Invoke();
        FetchPlayerList();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已加入房间\"" + PhotonNetwork.room.name + "\"，当前人数为" + PhotonNetwork.room.playerCount + "人。");
        if (PhotonNetwork.masterClient == null)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
            onBecomeMasterPlayer.Invoke();
        }
        else
        {
            onNotBecomeMasterPlayer.Invoke();
        }
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {            
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.player.Equals(newMasterClient))
        {
            onBecomeMasterPlayer.Invoke();
            onPhotonEvent.Invoke(ImprintLocalTime() + "已获得房主权限。");
        }
        else
        {
            onNotBecomeMasterPlayer.Invoke();
            onPhotonEvent.Invoke(ImprintLocalTime() + "玩家\"" + FetchPlayerName(newMasterClient) + "\"已成为房主。");
        }
            
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        FetchPlayerList();
        onPhotonEvent.Invoke(ImprintLocalTime() + "玩家\"" + FetchPlayerName(newPlayer) + "\"已加入房间，当前人数为" + PhotonNetwork.room.playerCount + "人。");
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        FetchPlayerList();
        onPhotonEvent.Invoke(ImprintLocalTime() + "玩家\"" + FetchPlayerName(otherPlayer) + "\"已离开房间，当前人数为" + PhotonNetwork.room.playerCount + "人。");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        onLeaveRoom.Invoke();
        onPhotonEvent.Invoke(ImprintLocalTime() + "已离开房间。");
    }        
    #endregion

    public static string FetchPlayerName(PhotonPlayer photonPlayer)
    {
        string anonymous = photonPlayer.ID > 0 ? string.Format(ANONYMOUS_FORMAT, ANONYMOUS, photonPlayer.ID) : ANONYMOUS;
        return string.IsNullOrEmpty(photonPlayer.name) ? anonymous : photonPlayer.name;
    }

    public void FetchRoomList()
    {
        RoomInfo[] roomInfo = PhotonNetwork.GetRoomList();
        OrderedDictionary dictionary = new OrderedDictionary();
        foreach (RoomInfo rI in roomInfo)
        {
            dictionary.Add(rI.name, rI);
        }
        onRoomListUpdate.Invoke(dictionary);
    }

    public void FetchPlayerList()
    {
        if (PhotonNetwork.room == null) { return; }

        PhotonPlayer[] photonPlayer = PhotonNetwork.playerList;
        OrderedDictionary dictionary = new OrderedDictionary();
        foreach (PhotonPlayer pP in photonPlayer)
        {
            dictionary.Add(pP.ID.ToString(), pP);
        }
        onPlayerListUpdate.Invoke(dictionary);
    }

    private bool ConnectToPUN()
    {
        onPhotonEvent.Invoke(ImprintLocalTime() + "正在连接PUN...");
        return PhotonNetwork.ConnectUsingSettings(m_PhotonIdentifier.GameIdentifier + '_' + m_PhotonIdentifier.GameVersion);
    }

    private IEnumerator ReconnectToPUN()
    {
        int counter = 0;
        bool isAttemptSuccessful = false;
        while (!isAttemptSuccessful)
        {
            onPhotonEvent.Invoke(ImprintLocalTime() + "正在重新连接PUN，尝试次数" + ++counter + "...");
            isAttemptSuccessful = PhotonNetwork.ReconnectAndRejoin();
            if (!isAttemptSuccessful)
            {
                isAttemptSuccessful = PhotonNetwork.Reconnect();
            }
            yield return new WaitForSeconds(SECONDS_BETWEEN_CONNECT_ATTEMPTS);
        }
    }

    private string ImprintLocalTime()
    {
        return string.Format("[{0:HH:mm:ss}]", DateTime.Now);
    }
}
