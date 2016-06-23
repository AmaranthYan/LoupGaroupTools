using Photon;
using UnityEngine;
using System.Collections;

public class ChatRoom : PunBehaviour
{
    [SerializeField]
    private NetLogger m_NetLogger = null;

    #region PhotonCallbacks
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ClearChatLog();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        ClearChatLog();
    }
    #endregion

    public void SendChatMessage(string chatMsg)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ReceiveChatMessage", PhotonTargets.All, "[" + NetPlayer.FetchPlayerName(PhotonNetwork.player) + "]:" + chatMsg);
    }

    [PunRPC]
    private void ReceiveChatMessage(string chatMsg)
    {
        m_NetLogger.AddMessage(chatMsg);
    }

    private void ClearChatLog()
    {
        m_NetLogger.Clear();
    }
}
