using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateNewRoomView : MonoBehaviour
{
    private const int MAX_PLAYER_TTL_MILLISECONDS = 5000;

    [SerializeField]
    private Text m_RoomName = null;
    [SerializeField]
    private Text m_MaxPlayers = null;

    public void CreateNewRoom()
    {
        if (!m_RoomName) { return; }
        if (!m_MaxPlayers) { return; }

        string name = !string.IsNullOrEmpty(m_RoomName.text) ? m_RoomName.text : null;
        RoomOptions options = new RoomOptions();
        if (!byte.TryParse(m_MaxPlayers.text, out options.MaxPlayers)) { options.MaxPlayers = 0; };
        options.PlayerTtl = MAX_PLAYER_TTL_MILLISECONDS;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        options.IsOpen = true;
        options.IsVisible = true;
        PhotonNetwork.CreateRoom(name, options, TypedLobby.Default);
    }
}
