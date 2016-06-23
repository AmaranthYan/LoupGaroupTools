namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;

    public class CharacterSettingView : ScrollListItemView
    {
        [Header("Events")]
        public UnityTypedEvent.SpriteEvent onCharacterSettingIconUpdate = new UnityTypedEvent.SpriteEvent();
        public UnityTypedEvent.StringEvent onCharacterSettingNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.BoolEvent onDefaultEnabledStateLoad = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.StringEvent onDefaultAmountLoad = new UnityTypedEvent.StringEvent();

        private CharacterModel m_Character = null;
        public CharacterModel Character { get { return m_Character; } }

        private CharacterSetting m_CharacterSetting = null;

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Character = (CharacterModel)value;
            onCharacterSettingIconUpdate.Invoke(m_Character.Icon);
            onCharacterSettingNameUpdate.Invoke(m_Character.DisplayName);

            LoadFromDefaultCharacterSetting();
        }

        private void LoadFromDefaultCharacterSetting()
        {
            m_CharacterSetting = SettingCacheService.Instance.LoadCharacterSettingFromCache(m_Character.Id);
            if (m_CharacterSetting != null)
            {
                onDefaultEnabledStateLoad.Invoke(m_CharacterSetting.IsEnabled);
                onDefaultAmountLoad.Invoke(m_CharacterSetting.Amount.ToString());
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
            SaveToDefaultCharacterSetting();
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
                SaveToDefaultCharacterSetting();
            }
        }

        private void SaveToDefaultCharacterSetting()
        {            
            SettingCacheService.Instance.SaveCharacterSettingToCache(m_CharacterSetting);
        }
    }
}
