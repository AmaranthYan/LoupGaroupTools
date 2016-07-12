using Photon;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class BulletinBoard : PunBehaviour
{
    [SerializeField]
    private Toggle m_EditToggle = null;
    [SerializeField]
    private Text m_OtherEditorInfoDisplay = null;
    [SerializeField]
    private string m_OtherEditorInfoFormat = string.Empty;
    [SerializeField]
    private Text m_BulletinDisplay = null;
    [SerializeField]
    private InputField m_BulletinEditor = null;    

    public UnityEvent onNoOneEdit = new UnityEvent();
    public UnityEvent onMeEdit = new UnityEvent();
    public UnityTypedEvent.PhotonPhotonPlayerEvent onOtherEditorEdit = new UnityTypedEvent.PhotonPhotonPlayerEvent();

    private PhotonView m_PhotonView = null;
    private PhotonPlayer m_CurrentEditor = null;

    void Awake()
    {
        m_PhotonView = PhotonView.Get(this);
    }

    #region PhotonCallbacks
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);

        if (PhotonNetwork.isMasterClient)
        {
            if (m_CurrentEditor.Equals(otherPlayer))
            {
                ReleaseEditorialAccess(otherPlayer);
            }
        }
    }
    #endregion

    public void InitBulletin(string initText)
    {
        m_BulletinDisplay.text = initText;
    }

    public void SwitchToEidtMode(bool isInEditMode)
    {
        if (isInEditMode)
        {
            RequestEditorialAccess();
        }
        else
        {
            ReleaseEditorialAccess();
        }
    }

    public void RequestEditorialAccess()
    {
        m_PhotonView.RPC("RequestEditorialAccess", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    private void RequestEditorialAccess(PhotonPlayer requestor)
    {
        if (m_CurrentEditor == null || m_CurrentEditor.Equals(requestor))
        {
            m_CurrentEditor = requestor;
            m_PhotonView.RPC("GrantEditorialAccess", PhotonTargets.AllViaServer, m_CurrentEditor);
        }
    }

    public void ReleaseEditorialAccess()
    {
        m_PhotonView.RPC("UpdateBulletinDisplay", PhotonTargets.All, m_BulletinEditor.text);
        m_BulletinEditor.gameObject.SetActive(false);

        m_PhotonView.RPC("ReleaseEditorialAccess", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    private void UpdateBulletinDisplay(string text)
    {
        m_BulletinDisplay.text = text;
    }

    [PunRPC]
    private void ReleaseEditorialAccess(PhotonPlayer editor)
    {
        if (m_CurrentEditor != null && m_CurrentEditor.Equals(editor))
        {
            m_CurrentEditor = null;
            m_PhotonView.RPC("WithdrawEditorialAccess", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    private void GrantEditorialAccess(PhotonPlayer editor)
    {
        if (PhotonNetwork.player.Equals(editor))
        {
            m_BulletinEditor.text = m_BulletinDisplay.text;
            m_BulletinEditor.gameObject.SetActive(true);
            m_OtherEditorInfoDisplay.gameObject.SetActive(false);
            onMeEdit.Invoke();
        }
        else
        {
            m_EditToggle.interactable = false;
            m_EditToggle.isOn = false;
            m_OtherEditorInfoDisplay.text = string.Format(m_OtherEditorInfoFormat, NetPlayer.FetchPlayerName(editor));
            m_OtherEditorInfoDisplay.gameObject.SetActive(true);
            onOtherEditorEdit.Invoke(editor);
        }
    }

    [PunRPC]
    private void WithdrawEditorialAccess()
    {
        m_EditToggle.isOn = false;
        m_EditToggle.interactable = true;
        m_OtherEditorInfoDisplay.gameObject.SetActive(false);
        onNoOneEdit.Invoke();
    }
}
