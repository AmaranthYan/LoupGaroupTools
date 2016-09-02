using UnityEngine;
using UnityEngine.Events;
using System;

public class KeyCodeRecorder : MonoBehaviour
{
    public UnityTypedEvent.KeyCodeEvent onKeyRecord = new UnityTypedEvent.KeyCodeEvent();
    public UnityEvent onRecordingStart = new UnityEvent();
    public UnityEvent onRecordingEnd = new UnityEvent();

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            onKeyRecord.Invoke(e.keyCode);
            EndRecording();
        }
        else if (e.isMouse)
        {
            EndRecording();
        }
    }

    public void StartRecording()
    {
        this.enabled = true;
        onRecordingStart.Invoke();
    }

    public void EndRecording()
    {
        onRecordingEnd.Invoke();
        this.enabled = false;
    }
}