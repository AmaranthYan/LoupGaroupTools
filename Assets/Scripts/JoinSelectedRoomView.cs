using UnityEngine;
using System.Collections;

public class JoinSelectedRoomView : MonoBehaviour {
    private string m_RoomName = null;
    public string RoomName { set { m_RoomName = value; } }

    public void JoinSelectedRoom()
    {
        if (string.IsNullOrEmpty(m_RoomName))
        {
            Debug.Log("房间名称为空。");
            return;
        }
        PhotonNetwork.JoinRoom(m_RoomName);
    }
}
