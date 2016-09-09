using Photon;
using UnityEngine;
using System.Collections;

public class VoiceInstance : MonoBehaviour
{
    [Header("Events")]
    public UnityTypedEvent.StringEvent onLogUpdate = new UnityTypedEvent.StringEvent();

    private NetVoice m_NetVocie = null;
    private PhotonView m_PhotonView = null;
    private PhotonVoiceRecorder m_VoiceRecorder = null;
    private PhotonVoiceSpeaker m_VoiceSpeaker = null;

    private bool isLocal = false;

    void Awake()
    {
        m_NetVocie = FindObjectOfType<NetVoice>();
        m_PhotonView = this.GetComponent<PhotonView>();
        m_VoiceRecorder = this.GetComponent<PhotonVoiceRecorder>();
        m_VoiceSpeaker = this.GetComponent<PhotonVoiceSpeaker>();
    }

    void Start()
    {
        this.transform.SetParent(m_NetVocie.transform);
        isLocal = m_PhotonView.owner.isLocal;
    }

    void OnDestroy()
    {
        // remove instance UI
    }

    void Update()
    {
        if (isLocal)
        {
            // add/update/remove instance UI
            // use Recoder.isTransmitting
        }
        else
        {
            // add/update/remove instance UI
            // use Speaker.isSpeaking
        }
    }
}