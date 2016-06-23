using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerView : ScrollListItemView<PhotonPlayer>
{
    [SerializeField]
    private SetMasterPlayerView m_SetMasterPlayerView = null;

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPlayerInfoUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.BoolEvent onPlayerStateUpdate = new UnityTypedEvent.BoolEvent();

    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_Item = (PhotonPlayer)value;
        if (m_SetMasterPlayerView)
        {
            m_SetMasterPlayerView.InitView(m_Item);
            m_SetMasterPlayerView.UpdateView();
        }
        onPlayerInfoUpdate.Invoke(NetPlayer.FetchPlayerName(m_Item));                
    }
}
