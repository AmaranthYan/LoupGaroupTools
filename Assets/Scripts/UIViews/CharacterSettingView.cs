namespace LoupsGarous
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class CharacterSettingView : ScrollListItemView<DataPair<CharacterModel, CharacterSetting>>
    {
        [Header("Events")]
        public UnityTypedEvent.SpriteEvent onCharacterSettingIconUpdate = new UnityTypedEvent.SpriteEvent();
        public UnityTypedEvent.StringEvent onCharacterSettingNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.BoolEvent onLocalEnabledStateLoad = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.StringEvent onLocalAmountLoad = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.StringEvent onWrongAmountInput = new UnityTypedEvent.StringEvent();

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
            if (m_Item.Value2 == null)
            {
                m_Item.Value2 = new CharacterSetting() { Id = m_Item.Value1.Id, Name = m_Item.Value1.DisplayName, IsEnabled = false, Amount = 0 };                
            }
            else if (!ValidateAmount(m_Item.Value2.Amount))
            {
                m_Item.Value2.Amount = 0;
            }
            onLocalEnabledStateLoad.Invoke(m_Item.Value2.IsEnabled);
            onLocalAmountLoad.Invoke(m_Item.Value2.Amount.ToString());
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
            int.TryParse(amountText, out amount);
            if (ValidateAmount(amount))
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
            else
            {
                onWrongAmountInput.Invoke(m_Item.Value2.Amount.ToString());
            }
        }

        private void StoreLocalCharacterSetting()
        {
            SettingStorageService.Instance.StoreCharacterSetting(m_Item.Value2);
        }

        private bool ValidateAmount(int amount)
        {
            return Regex.IsMatch(amount.ToString(), m_Item.Value1.AmountRegex);
        }
    }
}
