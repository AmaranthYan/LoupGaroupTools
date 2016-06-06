using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetServer : MonoBehaviour
{
    public string m_HostName = "God";
    public int m_Port = 25002;

    [SerializeField]
    private NetworkView m_NetworkView = null;

    void Awake()
    {
        //Network.Connect("https://www.insa-lyon.fr/");
    }

	void Start()
    {
        
    }

    void OnDestroy()
    {
        //Network.Disconnect();
    }
	
	void Update()
    {
        //Debug.Log(Network.HavePublicAddress());
        //Debug.Log(Network.player.externalIP);
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,20), "Init"))
        {
            // Use NAT punchthrough if no public IP present
            bool useNat = !Network.HavePublicAddress();
            Debug.Log(Network.InitializeServer(32, m_Port, useNat));
            //NetworkView networkView;
            m_NetworkView.RPC("AssignIdentity", Network.connections[0], "wo shi yyx");
        }

        if (Network.peerType == NetworkPeerType.Server)
        {
            GUI.Label(new Rect(0,30,100,20), "Server init");
        }
    }

    void OnServerInitialized()
    {
        MasterServer.RegisterHost("loupgarouptools_by_amaranth", m_HostName, "null");
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        Debug.Log(msEvent);
    }
}
