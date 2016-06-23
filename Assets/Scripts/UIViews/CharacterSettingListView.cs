namespace LoupsGarous
{
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;

    public class CharacterSettingListView : ScrollListView<CharacterModel>
    {
        [Serializable]
        public class SingleItemEvent : UnityEvent<CharacterModel> { }
        [Serializable]
        public class MultiItemEvent : UnityEvent<List<CharacterModel>> { }

        public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
        public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();
    }
}
