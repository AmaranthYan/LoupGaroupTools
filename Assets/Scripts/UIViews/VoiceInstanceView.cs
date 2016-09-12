using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class VoiceInstanceView : ScrollListItemView<VoiceInstanceModel>
{
    [SerializeField]
    private string m_InGameNumberFormat = string.Empty;
    [SerializeField]
    private Color m_DefaultColor = Color.white;
    [SerializeField]
    private Color m_IsLocalColor = Color.white;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onVoiceInstanceDisplayUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.ColorEvent onVoiceInstanceIsLocal = new UnityTypedEvent.ColorEvent();

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_Item = (VoiceInstanceModel)value;
        onVoiceInstanceIsLocal.Invoke(m_Item.IsLocal ? m_IsLocalColor : m_DefaultColor);
        string display = m_Item.IsInGame ? string.Format(m_InGameNumberFormat, m_Item.InGamePlayerNumber) : string.Empty;
        display += NetPlayer.FetchPlayerName(m_Item.Player);
        onVoiceInstanceDisplayUpdate.Invoke(display);      
    }
}
