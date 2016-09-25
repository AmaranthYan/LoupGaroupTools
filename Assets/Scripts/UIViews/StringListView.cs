using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class StringListView: ScrollListView<string>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<string> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<string>> { }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

    protected override void Awake()
    {
        base.onSelectedItemChange = onSelectedItemChange;
        base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
    }
}
