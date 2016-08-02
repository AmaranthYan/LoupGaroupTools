namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;
    using System.Linq;

    public class PlayerIdentityView : ScrollListItemView<PlayerIdentity>
    {
        [SerializeField]
        private CharacterDatabase m_CharacterDatabase = null;
        [SerializeField]
        private string m_UnknownPlayerName = string.Empty;

        [Header("Events")]
        public UnityTypedEvent.StringEvent onPlayerNameUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.StringEvent onPlayerNumberUpdate = new UnityTypedEvent.StringEvent();
        public UnityTypedEvent.SpriteEvent onIdentityImageUpdate = new UnityTypedEvent.SpriteEvent();
        public UnityTypedEvent.BoolEvent onIsMaster = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.BoolEvent onIsMe = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.BoolEvent onIsDead = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.BoolEvent onIsCaptain = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.BoolEvent onIdentityHide = new UnityTypedEvent.BoolEvent();

        void Start()
        {
            onIsMaster.Invoke(PhotonNetwork.isMasterClient);
        }

        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Item = (PlayerIdentity)value;
            onPlayerNameUpdate.Invoke(m_Item.Player != null ? NetPlayer.FetchPlayerName(m_Item.Player) : m_UnknownPlayerName);
            onPlayerNumberUpdate.Invoke(m_Item.Number > 0 ? m_Item.Number.ToString() : string.Empty);
            onIdentityImageUpdate.Invoke(m_Item.CharacterId != -1 ? m_CharacterDatabase.CharacterModels.First(c => c.Id == m_Item.CharacterId).Image : null);
            onIsMe.Invoke(PhotonNetwork.player.Equals(m_Item.Player));
            onIsDead.Invoke(m_Item.IsDead);
            onIsCaptain.Invoke(m_Item.IsCaptain);
            onIdentityHide.Invoke(!m_Item.IsRevealed);
        }
    }
}