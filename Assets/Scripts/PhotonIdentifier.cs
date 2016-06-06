using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "PhotonIdentifierSettings", menuName = "Photon/PhotonIdentifier", order = 1)]
public class PhotonIdentifier : ScriptableObject {
    [SerializeField]
    private string m_GameIdentifier = string.Empty;
    [SerializeField]
    private string m_GameVersion = string.Empty;

    public string GameIdentifier { get { return m_GameIdentifier; } }
    public string GameVersion { get { return m_GameVersion; } }
}
