namespace LoupsGarous
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class CharacterSettingView : ScrollListItemView<DataPair<CharacterModel, CharacterSetting>>
    {
        [Header("Events")]
        public UnityTypedEvent.SpriteEvent onCharacterSettingIconUpdate = new UnityTypedEvent.SpriteEvent();
        public UnityTypedEvent.StringEvent onCharacterSettingNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.BoolEvent onLocalEnabledStateLoad = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.StringEvent onLocalAmountLoad = new UnityTypedEvent.StringEvent();

        //private CharacterSetting m_CharacterSetting = null;

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Item = new DataPair<CharacterModel, CharacterSetting>((CharacterModel)value, null);
            onCharacterSettingIconUpdate.Invoke(m_Item.Value1.Icon);
            onCharacterSettingNameUpdate.Invoke(m_Item.Value1.DisplayName);

            RetrieveLocalCharacterSetting();
        }

        private void RetrieveLocalCharacterSetting()
        {
            m_Item.Value2 = SettingStorageService.Instance.RetrieveCharacterSetting(m_Item.Value1.Id);
            if (m_Item.Value2 != null)
            {
                onLocalEnabledStateLoad.Invoke(m_Item.Value2.IsEnabled);
                onLocalAmountLoad.Invoke(m_Item.Value2.Amount.ToString());
            }
        }

        public void SaveEnabledState(bool isEnabled)
        {
            if (m_Item.Value2 != null)
            {
                m_Item.Value2.IsEnabled = isEnabled;
            }
            else
            {
                m_Item.Value2 = new CharacterSetting() { Id = m_Item.Value1.Id, Name = m_Item.Value1.DisplayName, IsEnabled = isEnabled, Amount = 0 };
            }
            StoreLocalCharacterSetting();
        }

        public void SaveAmount(string amountText)
        {
            int amount;
            if (int.TryParse(amountText, out amount))
            {
                if (m_Item.Value2 != null)
                {
                    m_Item.Value2.Amount = amount;
                }
                else
                {
                    m_Item.Value2 = new CharacterSetting() { Id = m_Item.Value1.Id, Name = m_Item.Value1.DisplayName, IsEnabled = true, Amount = amount };
                }
                StoreLocalCharacterSetting();
            }
        }

        private void StoreLocalCharacterSetting()
        {
            SettingStorageService.Instance.StoreCharacterSetting(m_Item.Value2);
        }
    }
}
