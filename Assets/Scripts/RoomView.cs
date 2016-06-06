using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomView : MonoBehaviour {
    [SerializeField]
    private Toggle m_Toggle = null;
    [SerializeField]
    private string m_RoomStateFormat = string.Empty;

    private RoomListView m_RoomListView = null;
    public RoomListView RoomListView { get { return m_RoomListView; } set { m_RoomListView = value; } }
    private RoomInfo m_RoomInfo = null;
    public RoomInfo RoomInfo { get { return m_RoomInfo; } }

    [Header("Events")]
    public UnityTypedEvent.StringEvent onRoomNameUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.StringEvent onRoomStateUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.BoolEvent onRoomOpen = new UnityTypedEvent.BoolEvent();

    void OnDestroy()
    {
        IsSelectedInList(false);
    }

    public void InitRoomView(RoomListView roomListView)
    {
        m_RoomListView = roomListView;
        if (m_Toggle) { m_Toggle.group = roomListView.ToggleGroup; }
    }

    public void UpdateRoomInfo(RoomInfo roomInfo)
    {
        m_RoomInfo = roomInfo;
        onRoomNameUpdate.Invoke(roomInfo.name);
        onRoomStateUpdate.Invoke(string.Format(m_RoomStateFormat, roomInfo.playerCount, roomInfo.maxPlayers == 0 ? "无限" : roomInfo.maxPlayers.ToString(), roomInfo.open ? "开放" : "锁定"));
        onRoomOpen.Invoke(roomInfo.open);
    }    

    public void IsSelectedInList(bool isSelected)
    {
        if (m_RoomListView && (m_RoomInfo != null)) { m_RoomListView.SelectRoomInList(m_RoomInfo.name, isSelected); }
    }
}
