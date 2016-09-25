using UnityEngine;

public class StringView : ScrollListItemView<string>
{
    [Header("Events")]
    public UnityTypedEvent.StringEvent onStringUpdate = new UnityTypedEvent.StringEvent();

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_Item = (string)value;
        onStringUpdate.Invoke(m_Item);      
    }
}
