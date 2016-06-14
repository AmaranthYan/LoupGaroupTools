using Photon;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SetMasterPlayerView : PunBehaviour
{
    [SerializeField]
    private RectTransform m_IsMaster = null;
    [SerializeField]
    private RectTransform m_SetMasterButton = null;

    private PhotonPlayer m_PhotonPlayer = null;
    
    #region PhotonCallbacks
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        UpdateView();
    }
    #endregion
    public void InitView(PhotonPlayer photonPlayer)
    {
        m_PhotonPlayer = photonPlayer;
    }

    public void UpdateView()
    {
        m_IsMaster.gameObject.SetActive((m_PhotonPlayer != null) && m_PhotonPlayer.Equals(PhotonNetwork.masterClient));
        m_SetMasterButton.gameObject.SetActive(PhotonNetwork.player.isMasterClient && (m_PhotonPlayer != null) && !m_PhotonPlayer.Equals(PhotonNetwork.masterClient));
    }

    public void SetMasterPlayer()
    {
        if (m_PhotonPlayer == null) { return; }

        if (PhotonNetwork.inRoom && PhotonNetwork.player.isMasterClient)
        {
            PhotonNetwork.SetMasterClient(m_PhotonPlayer);
        }
    }
}
