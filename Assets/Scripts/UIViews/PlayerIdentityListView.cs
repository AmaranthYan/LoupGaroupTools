namespace LoupsGarous
{
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    public class PlayerIdentityListView : ScrollListView<PlayerIdentity>
    {
        [Serializable]
        public class SingleItemEvent : UnityEvent<PlayerIdentity> { }
        [Serializable]
        public class MultiItemEvent : UnityEvent<List<PlayerIdentity>> { }

        public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
        public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

        protected override void Awake()
        {
            base.onSelectedItemChange = onSelectedItemChange;
            base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
        }
    }
}