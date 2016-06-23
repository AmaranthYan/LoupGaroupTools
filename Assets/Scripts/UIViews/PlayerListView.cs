using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class PlayerListView : ScrollListView<PhotonPlayer>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<PhotonPlayer> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<PhotonPlayer>> { }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();
}
