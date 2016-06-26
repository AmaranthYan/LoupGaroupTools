namespace LoupsGarous
{
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    public class CharacterSettingListView : ScrollListView<DataPair<CharacterModel, CharacterSetting>>
    {
        [Serializable]
        public class SingleItemEvent : UnityEvent<DataPair<CharacterModel, CharacterSetting>> { }
        [Serializable]
        public class MultiItemEvent : UnityEvent<List<DataPair<CharacterModel, CharacterSetting>>> { }

        public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
        public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

        protected override void Awake()
        {
            base.onSelectedItemChange = onSelectedItemChange;
            base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
        }
    }
}
