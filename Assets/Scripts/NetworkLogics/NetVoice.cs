using Photon;
using UnityEngine;
using System;

public class NetVoice : PunBehaviour
{
    public enum RecordingMode { PushToTalk = 0, FreeMic }

    private const string VOICE_MODE_KEY = "voice_mode";
    private const string PTT_KEYCODE_KEY = "ptt_keycode";

    [Header("Prefabs")]
    [SerializeField]
    private string m_VoiceInstancePrefabName = string.Empty;
    [Header("UI")]
    [SerializeField]
    private VoiceInstanceListView m_VoiceInstanceListView = null;
    public VoiceInstanceListView VoiceInstanceListView { get { return m_VoiceInstanceListView; } }

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPhotonEvent = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.IntEvent onModeInit = new UnityTypedEvent.IntEvent();
    public UnityTypedEvent.StringEvent onPTTKeyUpdate = new UnityTypedEvent.StringEvent();

    private RecordingMode m_Mode = RecordingMode.PushToTalk;
    private KeyCode m_PTTKey = KeyCode.Return;

    private string m_Microphone = null;

    private static GameObject m_LocalVoiceInstance = null;
    public static GameObject LocalVoiceInstance { get { return m_LocalVoiceInstance; } }

    void Start()
    {
        m_Mode = (RecordingMode)(PlayerPrefs.GetInt(VOICE_MODE_KEY, (int)m_Mode));
        onModeInit.Invoke((int)m_Mode);

        m_PTTKey = (KeyCode)(PlayerPrefs.GetInt(PTT_KEYCODE_KEY, (int)m_PTTKey));
        onPTTKeyUpdate.Invoke(m_PTTKey.ToString());
    }

    void FixedUpdate()
    {
        DetectMicrophone();
        ValidateLocalVoiceTransmission();        
    }

    #region PhotonCallbacks
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (string.IsNullOrEmpty(m_VoiceInstancePrefabName))
        {
            Debug.LogError("预制语音实例为空，无法生成语音实例！");
            return;
        }

        m_LocalVoiceInstance = PhotonNetwork.Instantiate(m_VoiceInstancePrefabName, Vector3.zero, Quaternion.identity, 0);
        // todo : parenting of other players voice instance
        m_LocalVoiceInstance.transform.SetParent(transform);
        m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().Transmit = false;
        m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().MicrophoneDevice = m_Microphone;
        onPhotonEvent.Invoke(Timestamp.ImprintLocalTime() + "已加入房间语音信道" + (m_Microphone == null ? ",当前无可用麦克风。" : "，当前麦克风" + m_Microphone));
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        if (!m_LocalVoiceInstance) { return; }
            
        m_LocalVoiceInstance = null;
        onPhotonEvent.Invoke(Timestamp.ImprintLocalTime() + "已离开房间语音信道");
    }
    #endregion

    public void SetCurrentMode(int index)
    {
        switch ((RecordingMode)index)
        {
            case RecordingMode.PushToTalk:
                m_Mode = RecordingMode.PushToTalk;
                break;
            case RecordingMode.FreeMic:
                m_Mode = RecordingMode.FreeMic;
                break;
            default:
                Debug.LogError("无效的语音模式！");
                return;
        }
        PlayerPrefs.SetInt(VOICE_MODE_KEY, (int)m_Mode);
    }

    public void SetPTTKey(KeyCode newKey)
    {
        m_PTTKey = newKey;
        onPTTKeyUpdate.Invoke(m_PTTKey.ToString());
        PlayerPrefs.SetInt(PTT_KEYCODE_KEY, (int)m_PTTKey);
    }

    private void DetectMicrophone()
    {
        m_Microphone = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        if (m_Microphone == null) { return; }

        if (m_LocalVoiceInstance && m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().MicrophoneDevice == null)
        {
            m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().MicrophoneDevice = m_Microphone;
        }
    }

    private void ValidateLocalVoiceTransmission()
    {
        if (!m_LocalVoiceInstance) { return; }

        switch (m_Mode)
        {
            case RecordingMode.PushToTalk:
                m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().Transmit = Input.GetKey(m_PTTKey);                
                break;
            case RecordingMode.FreeMic:
                m_LocalVoiceInstance.GetComponent<PhotonVoiceRecorder>().Transmit = true;
                break;
        }
    }
}