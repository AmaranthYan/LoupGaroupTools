using UnityEngine;
using UnityEngine.Events;

public class GameVersionView : MonoBehaviour
{
    [SerializeField]
    private PhotonIdentifier m_PhotonIdentifierSettings = null;
    [SerializeField]
    private string m_GameVersionFormat = string.Empty;

    public UnityTypedEvent.StringEvent onGameVersionInfoLoad = new UnityTypedEvent.StringEvent();

    void Start()
    {
        if (m_PhotonIdentifierSettings)
        {
            onGameVersionInfoLoad.Invoke(string.Format(m_GameVersionFormat, m_PhotonIdentifierSettings.GameVersion));
        }
    }
}
