namespace LoupsGarous
{
    using UnityEngine;
    using System.Collections;

    public class CharacterSettingView : ScrollListItemView
    {
        [Header("Events")]
        public UnityTypedEvent.SpriteEvent onCharacterSettingIconUpdate = new UnityTypedEvent.SpriteEvent();
        public UnityTypedEvent.StringEvent onCharacterSettingNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.BoolEvent onLocalEnabledStateLoad = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.StringEvent onLocalAmountLoad = new UnityTypedEvent.StringEvent();

        private CharacterModel m_Character = null;
        public CharacterModel Character { get { return m_Character; } }

        private CharacterSetting m_CharacterSetting = null;

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Character = (CharacterModel)value;
            onCharacterSettingIconUpdate.Invoke(m_Character.Icon);
            onCharacterSettingNameUpdate.Invoke(m_Character.DisplayName);

            RetrieveLocalCharacterSetting();
        }

        private void RetrieveLocalCharacterSetting()
        {
            m_CharacterSetting = SettingStorageService.Instance.RetrieveCharacterSetting(m_Character.Id);
            if (m_CharacterSetting != null)
            {
                onLocalEnabledStateLoad.Invoke(m_CharacterSetting.IsEnabled);
                onLocalAmountLoad.Invoke(m_CharacterSetting.Amount.ToString());
            }
        }

        public void SaveEnabledState(bool isEnabled)
        {
            if (m_CharacterSetting != null)
            {
                m_CharacterSetting.IsEnabled = isEnabled;
            }
            else
            {
                m_CharacterSetting = new CharacterSetting() { Id = m_Character.Id, Name = m_Character.DisplayName, IsEnabled = isEnabled, Amount = 0 };
            }
            StoreLocalCharacterSetting();
        }

        public void SaveAmount(string amountText)
        {
            int amount;
            if (int.TryParse(amountText, out amount))
            {
                if (m_CharacterSetting != null)
                {
                    m_CharacterSetting.Amount = amount;
                }
                else
                {
                    m_CharacterSetting = new CharacterSetting() { Id = m_Character.Id, Name = m_Character.DisplayName, IsEnabled = true, Amount = amount };
                }
                StoreLocalCharacterSetting();
            }
        }

        private void StoreLocalCharacterSetting()
        {
            SettingStorageService.Instance.StoreCharacterSetting(m_CharacterSetting);
        }
    }
}
