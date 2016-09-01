using Photon;
using UnityEngine.UI;
using System;

public class Voice : PunBehaviour
{
	public enum RecordingMode { PushToTalk = 0, FreeMic }

    private const string VOICE_MODE_KEY = "voice_mode";
    private const string PTT_KEYCODE_KEY = "ptt_keycode";

    public UnityTypedEvent.IntEvent onModeUpdate = new UnityTypedEvent.IntEvent();
    public UnityTypedEvent.StringEvent onPTTKeyUpdate = new UnityTypedEvent.StringEvent();

    private RecordingMode m_Mode = RecordingMode.PushToTalk;
    private KeyCode m_PTTKey = KeyCode.Return;

    void Start()
    {
        m_Mode = (RecordingMode)(PlayerPrefs.GetInt(VOICE_MODE_KEY, (int)m_Mode));
        onModeUpdate.Invoke((int)m_Mode);

        m_PTTKey = (KeyCode)(PlayerPrefs.GetInt(PTT_KEYCODE_KEY, (int)m_PTTKey));
        onPTTKeyUpdate.Invoke(m_PTTKey.ToString());
    }

    void Update()
    {
        switch ((RecordingMode)index)
        {
            case RecordingMode.PushToTalk :
                //todo
                break;
            case RecordingMode.FreeMic :                
                break;
        }
    }

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
                Debug.LogErr("无效的语音模式！");
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
