using UnityEngine;
using System.Collections;

public class JoinSelectedRoomView : MonoBehaviour
{
    private RoomView m_RoomView = null;
    public RoomView RoomView { set { m_RoomView = value; } }

    public void SetRoomView(ScrollListItemView roomView)
    {
        m_RoomView = (RoomView)roomView;
    }

    public void JoinSelectedRoom()
    {
        if (!m_RoomView)
        {
            Debug.Log("房间名称为空。");
            return;
        }
        PhotonNetwork.JoinRoom(m_RoomView.RoomInfo.name);
    }
}
