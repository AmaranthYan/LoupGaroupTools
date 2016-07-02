using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[RequireComponent(typeof(ToggleGroup))]
public class TabGroupView : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup m_TabGroup = null;
    [SerializeField]
    private Toggle[] m_TabsInGroup = null;

    void Awake()
    {
        foreach (Toggle tab in m_TabsInGroup)
        {
            m_TabGroup.RegisterToggle(tab);
        }
    }

    public void EnableTab(string name)
    {
        if (!m_TabGroup) { return; }

        Toggle tab = m_TabsInGroup.First(t => t.name == name);
        if (!tab)
        {
            Debug.LogWarning("未找到" + name + "标签。");
            return;
        }
        tab.interactable = true;
    }

    public void DisableTab(string name)
    {
        if (!m_TabGroup) { return; }

        Toggle tab = m_TabsInGroup.First(t => t.name == name);
        if (!tab)
        {
            Debug.LogWarning("未找到" + name + "标签。");
            return;
        }
        tab.interactable = false;

        Toggle firstInteractable = null;
        for (int i = 0; i < m_TabsInGroup.Length; i++)
        {
            if (m_TabsInGroup[i].IsInteractable())
            {
                firstInteractable = m_TabsInGroup[i];
                break;
            }
        }
        if (!m_TabGroup.allowSwitchOff)
        {
            if (!firstInteractable)
            {
                Debug.LogError("没有任何可选中的标签，操作未完成。");
                tab.interactable = true;
                return;
            }
            firstInteractable.isOn = true;
            firstInteractable.onValueChanged.Invoke(firstInteractable.isOn);
        }
        tab.isOn = false;
        tab.onValueChanged.Invoke(tab.isOn);
    }
}
