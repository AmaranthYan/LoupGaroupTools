using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupIndexer : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup m_ToggleGroup = null;
    [SerializeField]
    private Toggle[] m_Toggles = new Toggle[0];

    public UnityTypedEvent.IntEvent onToggleOn = new UnityTypedEvent.IntEvent();

    void Awake()
    {
        m_ToggleGroup = this.GetComponent<ToggleGroup>();
        foreach (Toggle toggle in m_Toggles)
        {
            toggle.group = m_ToggleGroup;
        }
    }

    void Start()
    {
        for (int i = 0; i < m_Toggles.Length; i++)
        {
            int idx = i;
            m_Toggles[i].onValueChanged.AddListener((bool b) => 
            {
                if (b) {
                    onToggleOn.Invoke(idx);
                }
            });
        }
    }

    public void SetToggleOn(int index)
    {
        if (index < 0 || index >= m_Toggles.Length) { return; }

        m_Toggles[index].isOn = true;
        m_ToggleGroup.NotifyToggleOn(m_Toggles[index]);
    }
}
