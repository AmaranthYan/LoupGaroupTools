using System.Xml.Serialization;

public class CharacterSetting
{
    private int m_Id;
    private string m_Name;
    private bool m_IsEnabled;
    private int m_Amount;

    [XmlAttribute("id")]
    public int Id { get { return m_Id; } set { m_Id = value; } }
    [XmlAttribute("name")]
    public string Name { get { return m_Name; } set { m_Name = value; } }
    public bool IsEnabled { get { return m_IsEnabled; } set { m_IsEnabled = value; } }
    public int Amount { get { return m_Amount; } set { m_Amount = value; } }
}
