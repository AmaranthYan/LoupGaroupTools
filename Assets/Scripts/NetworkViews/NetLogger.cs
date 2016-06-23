using UnityEngine;
using System.Collections;

public class NetLogger : MonoBehaviour
{
    private string Log = string.Empty;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onLogUpdate = new UnityTypedEvent.StringEvent();

    public void AddMessage(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            Log = msg + (string.IsNullOrEmpty(Log) ? string.Empty : '\n' + Log);
            onLogUpdate.Invoke(Log);
        }        
    }

    public void Clear()
    {
        Log = string.Empty;
        onLogUpdate.Invoke(Log);
    }
}
