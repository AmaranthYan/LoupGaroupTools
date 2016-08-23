using System;
using System.Collections.Generic;

public class MessageChannelModel
{
    private int m_ChannelId;
    private string m_ChannelName;
    private HashSet<int> m_PlayerNumbers;

    public int ChannelId { get { return m_ChannelId; } }
    public string ChannelName { get { return m_ChannelName; } set { m_ChannelName = value; } }
    public HashSet<int> PlayerNumbers { get { return m_PlayerNumbers; } set { m_PlayerNumbers = value; } }

    public MessageChannelModel(int id)
    {
        m_ChannelId = id;
        m_ChannelName = string.Empty;
        m_PlayerNumbers = new HashSet<int>();
    }
}
