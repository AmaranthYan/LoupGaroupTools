using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class RoomView : ScrollListItemView
{
    [SerializeField]
    private string m_RoomStateFormat = string.Empty;

    private RoomInfo m_RoomInfo = null;
    public RoomInfo RoomInfo { get { return m_RoomInfo; } }

    [Header("Events")]
    public UnityTypedEvent.StringEvent onRoomNameUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.StringEvent onRoomStateUpdate = new UnityTypedEvent.StringEvent();
    public UnityEvent onRoomOpen = new UnityEvent();
    public UnityEvent onRoomClose = new UnityEvent();

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_RoomInfo = (RoomInfo)value;
        onRoomNameUpdate.Invoke(m_RoomInfo.name);
        onRoomStateUpdate.Invoke(string.Format(m_RoomStateFormat, m_RoomInfo.playerCount, m_RoomInfo.maxPlayers == 0 ? "无限" : m_RoomInfo.maxPlayers.ToString(), m_RoomInfo.open ? "开放" : "锁定"));
        if (m_RoomInfo.open)
        {
            onRoomOpen.Invoke();
        }
        else
        {
            onRoomClose.Invoke();
        }        
    }
}
