using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerView : ScrollListItemView
{
    [SerializeField]
    private SetMasterPlayerView m_SetMasterPlayerView = null;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPlayerInfoUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.BoolEvent onPlayerStateUpdate = new UnityTypedEvent.BoolEvent();

    private PhotonPlayer m_PhotonPlayer = null;
    public PhotonPlayer PhotonPlayer { get { return m_PhotonPlayer; } }

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_PhotonPlayer = (PhotonPlayer)value;
        if (m_SetMasterPlayerView)
        {
            m_SetMasterPlayerView.InitView(m_PhotonPlayer);
            m_SetMasterPlayerView.UpdateView();
        }
        onPlayerInfoUpdate.Invoke(NetPlayer.FetchPlayerName(m_PhotonPlayer));                
    }
}
