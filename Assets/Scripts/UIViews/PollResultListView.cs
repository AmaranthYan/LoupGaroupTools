namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class PollResultListView : ScrollListView<DataPair<PlayerIdentity, List<PlayerIdentity>>>
    {
        [Serializable]
        public class SingleItemEvent : UnityEvent<DataPair<PlayerIdentity, List<PlayerIdentity>>> { }
        [Serializable]
        public class MultiItemEvent : UnityEvent<List<DataPair<PlayerIdentity, List<PlayerIdentity>>>> { }
        
        public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
        public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();
        
        protected override void Awake()
        {
            base.onSelectedItemChange = onSelectedItemChange;
            base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
        }
    }
}