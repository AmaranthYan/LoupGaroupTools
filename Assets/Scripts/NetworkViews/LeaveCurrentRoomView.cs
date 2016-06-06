using UnityEngine;
using System.Collections;

public class LeaveCurrentRoomView : MonoBehaviour {
    public void LeaveCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
