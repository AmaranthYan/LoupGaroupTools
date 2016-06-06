using UnityEngine;
using UnityEngine.Networking;
using Photon;
using System.Collections;

public class NetClient : PunBehaviour
{
    public int m_ServerPort = 25002;
	
	void Start()
    {
        PhotonNetwork.ConnectUsingSettings("v1.0");
    }
	
	void Update()
    {
        
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(220, 0, 100, 20), "Room"))
        {
            RoomInfo[] roomInfo = PhotonNetwork.GetRoomList();
            foreach (RoomInfo rm in roomInfo)
            {
                Debug.Log(rm.name);
            }
            Debug.Log(PhotonNetwork.GetRoomList().Length);
        }

        if (GUI.Button(new Rect(330, 0, 100, 20), "CRoom"))
        {
            PhotonNetwork.CreateRoom(Time.realtimeSinceStartup.ToString());
            //Debug.Log(PhotonNetwork.GetRoomList());
        }

        if (GUI.Button(new Rect(110, 0, 100, 20), "Connect"))
        {
            MasterServer.RequestHostList("loupgarouptools_by_amaranth");
        }

        if (Network.peerType == NetworkPeerType.Client)
        {
            GUI.Label(new Rect(110, 30, 100, 20), "Client conn");
            //NetworkClient nc;
            //nc.Connect()
        }
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        switch (msEvent)
        {
            case MasterServerEvent.HostListReceived :
                HostData[] hostData = MasterServer.PollHostList();
                Debug.Log(hostData[0]);
                //"88.163.117.135"
                Debug.Log(hostData.Length > 0 ? Network.Connect(hostData[0]) : NetworkConnectionError.EmptyConnectTarget);
                break;
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(1);
        //Debug.Log(PhotonNetwork.CreateRoom(Time.realtimeSinceStartup.ToString()));
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(2);
    }
}
