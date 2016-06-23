using UnityEngine;
using System.Collections;

public class JoinSelectedRoomView : MonoBehaviour
{
    private RoomInfo m_SelectedRoom = null;
    public RoomInfo SelectedRoom { set { m_SelectedRoom = value; } }

    public void SetRoomView(RoomInfo roomInfo)
    {
        m_SelectedRoom = roomInfo;
    }

    public void JoinSelectedRoom()
    {
        if (m_SelectedRoom == null)
        {
            Debug.Log("房间名称为空。");
            return;
        }
        PhotonNetwork.JoinRoom(m_SelectedRoom.name);
    }
}
