using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetPlayerNameView : PunBehaviour
{
    private static void DefaultCallback() { }

    private const string PLAYER_PERSISTENT_NAME_KEY = "player_persistent_name";

    [SerializeField]
    private InputField m_PlayerNameField = null;

    private bool m_IsConnected = false;
    private NetPlayer.CustomPhotonCallback OnConnected_Callback = DefaultCallback;

    void Start()
    {
        string playerName = PlayerPrefs.GetString(PLAYER_PERSISTENT_NAME_KEY);
        OnConnected_Callback = () => { PhotonNetwork.playerName = playerName; };
        
        if (m_PlayerNameField)
        {
            m_PlayerNameField.text = playerName;
        }
    }

    #region PhotonCallbacks
    public override void OnConnectedToPhoton()
    {
        base.OnConnectedToPhoton();
        m_IsConnected = true;
        OnConnected_Callback();
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        m_IsConnected = false;
    }
    #endregion

    public void SetPlayerName(string playerName)
    {
        if (m_IsConnected)
        {
            PhotonNetwork.playerName = playerName;
        }
        else
        {
            OnConnected_Callback = () => { PhotonNetwork.playerName = playerName; };
        }
        PlayerPrefs.SetString(PLAYER_PERSISTENT_NAME_KEY, playerName);
    }
}
