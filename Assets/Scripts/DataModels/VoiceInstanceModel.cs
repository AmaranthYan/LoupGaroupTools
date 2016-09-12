using Photon;
using System;

public class VoiceInstanceModel
{
    private PhotonPlayer m_Player;
    private bool m_IsLocal;
    private bool m_IsInGame;
    private int m_InGamePlayerNumber;

    public PhotonPlayer Player { get { return m_Player; } set { m_Player = value; } }
    public bool IsLocal { get { return m_IsLocal; } set { m_IsLocal = value; } }
    public bool IsInGame { get { return m_IsInGame; } set { m_IsInGame = value; } }
    public int InGamePlayerNumber { get { return m_InGamePlayerNumber; } set { m_InGamePlayerNumber = value; } }
}
