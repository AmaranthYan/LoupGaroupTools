using UnityEngine;
using System.Collections;

public class SetPlayerNameView : MonoBehaviour
{
    public void SetPlayerName(string playerName)
    {
        PhotonNetwork.playerName = playerName;
    }
}
