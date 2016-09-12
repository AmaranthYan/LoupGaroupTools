using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class VoiceInstanceListView : ScrollListView<VoiceInstanceModel>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<VoiceInstanceModel> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<VoiceInstanceModel>> { }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

    protected override void Awake()
    {
        base.onSelectedItemChange = onSelectedItemChange;
        base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
    }
}
