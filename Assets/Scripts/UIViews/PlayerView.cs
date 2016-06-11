using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerView : ScrollListItemView
{
    [Header("Events")]
    public UnityTypedEvent.StringEvent onPlayerInfoUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.BoolEvent onPlayerStateUpdate = new UnityTypedEvent.BoolEvent();

    private PhotonPlayer m_PhotonPlayer = null;
    public PhotonPlayer PhotonPlayer { get { return m_PhotonPlayer; } }

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_PhotonPlayer = (PhotonPlayer)value;
        onPlayerInfoUpdate.Invoke(NetPlayer.FetchPlayerName(m_PhotonPlayer) + (m_PhotonPlayer.isMasterClient ? " (房主)" : string.Empty));                
    }
}
