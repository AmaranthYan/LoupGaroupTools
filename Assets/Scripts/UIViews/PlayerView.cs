using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerView : ScrollListItemView
{
    [SerializeField]
    private string m_PlayerStateFormat = string.Empty;

    private PhotonPlayer m_PhotonPlayer = null;
    public PhotonPlayer PhotonPlayer { get { return m_PhotonPlayer; } }

    [Header("Events")]
    public UnityTypedEvent.StringEvent onPlayerInfoUpdate = new UnityTypedEvent.StringEvent();
    public UnityTypedEvent.BoolEvent onPlayerStateUpdate = new UnityTypedEvent.BoolEvent();
    
    public override void UpdateItem(object value)
    {
        base.UpdateItem(value);
        m_PhotonPlayer = (PhotonPlayer)value;
        onPlayerInfoUpdate.Invoke(m_PhotonPlayer.ToString());                
    }
}
