using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputMessageView : MonoBehaviour
{
    [SerializeField]
    private InputField m_InputField;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onSendMessage = new UnityTypedEvent.StringEvent();

    public void SendMessage()
    {
        string msg = m_InputField.text;
        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogWarning("输入信息不能为空。");
            return;
        }

        onSendMessage.Invoke(msg);
        ClearInput();
    }

    private void ClearInput()
    {
        m_InputField.text = string.Empty;
    }
}
