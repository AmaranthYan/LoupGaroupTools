using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class RoomListView: ScrollListView<RoomInfo>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<RoomInfo> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<RoomInfo>> { }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();
}
