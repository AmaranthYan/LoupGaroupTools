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

        private CharacterModel m_Character = null;
        public CharacterModel Character { get { return m_Character; } }

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Character = (CharacterModel)value;
            onCharacterSettingIconUpdate.Invoke(m_Character.Icon);
            onCharacterSettingNameUpdate.Invoke(m_Character.DisplayName);
        }
    }
}
