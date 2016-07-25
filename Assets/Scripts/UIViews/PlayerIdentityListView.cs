namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class PlayerIdentityListView : ScrollListView<PlayerIdentity>
    {
        [Serializable]
        public class SingleItemEvent : UnityEvent<PlayerIdentity> { }
        [Serializable]
        public class MultiItemEvent : UnityEvent<List<PlayerIdentity>> { }

        [SerializeField]
        private RectTransform m_MePlaceholder = null;

        public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
        public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

        public UnityTypedEvent.BoolEvent onMePresent = new UnityTypedEvent.BoolEvent();

        private bool m_HasMe = false;

        protected override void Awake()
        {
            base.onSelectedItemChange = onSelectedItemChange;
            base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
        }

        public override void UpdateList(OrderedDictionary itemDictionary)
        {
            m_HasMe = false;
            base.UpdateList(itemDictionary);
            onMePresent.Invoke(m_HasMe);
        }

        protected override void UpdateItemView(ScrollListItemView<PlayerIdentity> itemView, object value)
        {
            base.UpdateItemView(itemView, value);
            PlayerIdentity playerIdentity = (PlayerIdentity)value;
            if (PhotonNetwork.player.Equals(playerIdentity.Player))
            {
                itemView.transform.SetParent(m_MePlaceholder);
                m_HasMe = true;
            }
        }
    }
}