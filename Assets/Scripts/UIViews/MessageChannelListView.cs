using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class MessageChannelListView : ScrollListView<MessageChannelModel>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<MessageChannelModel> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<MessageChannelModel>> { }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

    protected override void Awake()
    {
        base.onSelectedItemChange = onSelectedItemChange;
        base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
    }
}