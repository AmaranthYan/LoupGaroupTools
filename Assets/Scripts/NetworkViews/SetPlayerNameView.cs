using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetPlayerNameView : MonoBehaviour
{
    private const string PLAYER_PERSISTENT_NAME_KEY = "player_persistent_name";

    [SerializeField]
    private InputField m_PlayerNameField = null;

    void Start()
    {
        PhotonNetwork.player.userId = PhotonNetwork.player.userId ?? NetPlayer.PlayerGuid.ToString();
        PhotonNetwork.playerName = PlayerPrefs.GetString(PLAYER_PERSISTENT_NAME_KEY);       
        if (m_PlayerNameField)
        {
            m_PlayerNameField.text = PhotonNetwork.playerName;
        }        
    }

    public void SetPlayerName(string playerName)
    {
        PhotonNetwork.player.userId = PhotonNetwork.player.userId ?? NetPlayer.PlayerGuid.ToString();
        PhotonNetwork.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PERSISTENT_NAME_KEY, PhotonNetwork.playerName);
    }
}
