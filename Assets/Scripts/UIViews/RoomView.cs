using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class RoomView : ScrollListItemView<RoomInfo>
{
    [SerializeField]
    private string m_RoomStateFormat = string.Empty;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onRoomNameUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.StringEvent onRoomStateUpdate = new UnityTypedEvent.StringEvent();
    public UnityEvent onRoomOpen = new UnityEvent();
    public UnityEvent onRoomClose = new UnityEvent();

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_Item = (RoomInfo)value;
        onRoomNameUpdate.Invoke(m_Item.open ? m_Item.name : "<color=#808080>" + m_Item.name + "</color>");
        onRoomStateUpdate.Invoke(string.Format(m_Item.open ? m_RoomStateFormat : "<color=#808080>" + m_RoomStateFormat + "</color>", m_Item.playerCount, m_Item.maxPlayers == 0 ? "无限" : m_Item.maxPlayers.ToString(), m_Item.open ? "开放" : "锁定"));
        if (m_Item.open)
        {
            onRoomOpen.Invoke();
        }
        else
        {
            onRoomClose.Invoke();
        }        
    }
}
