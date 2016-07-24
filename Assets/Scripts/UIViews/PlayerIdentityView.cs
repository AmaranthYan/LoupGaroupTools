namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;

    public class PlayerIdentityView : ScrollListItemView<PlayerIdentity>
    {
        [SerializeField]
        private string m_UnknownPlayerName = string.Empty;

        [Header("Events")]
        public UnityTypedEvent.StringEvent onPlayerNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.StringEvent onPlayerNumberUpdate = new UnityTypedEvent.StringEvent();

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Item = (PlayerIdentity)value;
            onPlayerNameUpdate.Invoke(m_Item.Player != null ? NetPlayer.FetchPlayerName(m_Item.Player) : m_UnknownPlayerName);
            onPlayerNumberUpdate.Invoke(m_Item.Number > 0 ? m_Item.Number.ToString() : string.Empty);
        }
    }
}