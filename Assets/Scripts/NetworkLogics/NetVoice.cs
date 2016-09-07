using Photon;
using UnityEngine;
using System;

public class NetVoice : PunBehaviour
{
	public enum RecordingMode { PushToTalk = 0, FreeMic }

    private const string VOICE_MODE_KEY = "voice_mode";
    private const string PTT_KEYCODE_KEY = "ptt_keycode";

    [Header("Injections")]
    [SerializeField]
    private GameObject m_VoiceInstancePrefab = null;

    [Header("Events")]
    public UnityTypedEvent.IntEvent onModeInit = new UnityTypedEvent.IntEvent();
    public UnityTypedEvent.StringEvent onPTTKeyUpdate = new UnityTypedEvent.StringEvent();

    private RecordingMode m_Mode = RecordingMode.PushToTalk;
    private KeyCode m_PTTKey = KeyCode.Return;

    void Start()
    {
        m_Mode = (RecordingMode)(PlayerPrefs.GetInt(VOICE_MODE_KEY, (int)m_Mode));
        onModeInit.Invoke((int)m_Mode);

        m_PTTKey = (KeyCode)(PlayerPrefs.GetInt(PTT_KEYCODE_KEY, (int)m_PTTKey));
        onPTTKeyUpdate.Invoke(m_PTTKey.ToString());
    }

    void Update()
    {
        switch (m_Mode)
        {
            case RecordingMode.PushToTalk :
                //todo
                break;
            case RecordingMode.FreeMic :                
                break;
        }
    }

    #region PhotonCallbacks
    public override void OnJoinedRoom()
    {
        GameObject voiceInstance = PhotonNetwork.Instantiate("VoiceInstance", Vector3.zero, Quaternion.identity, 0);
        voiceInstance.transform.SetParent(transform);
        Debug.Log(Microphone.devices.Length);
        Debug.Log(voiceInstance.GetComponent<PhotonVoiceRecorder>().MicrophoneDevice);        
    }
    #endregion

    public void SetCurrentMode(int index)
    {        
        switch ((RecordingMode)index)
        {
            case RecordingMode.PushToTalk :
                m_Mode = RecordingMode.PushToTalk;
                break;
            case RecordingMode.FreeMic :
                m_Mode = RecordingMode.FreeMic;
                break;
            default :
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
}
