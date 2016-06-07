using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ToggleGroup))]
public class ScrollListView : MonoBehaviour
{
    [SerializeField]
    protected ToggleGroup m_ToggleGroup = null;
    public ToggleGroup ToggleGroup { get { return m_ToggleGroup; } }
    [SerializeField]
    protected ScrollListItemView m_ScrollListItemPrefab = null;

    protected Dictionary<string, ScrollListItemView> m_CurrentScrollList = new Dictionary<string, ScrollListItemView>();
    protected string m_SelectedRoom = null;

    public UnityTypedEvent.StringEvent onSelectedRoomChange = new UnityTypedEvent.StringEvent();

    public void UpdateList(Hashtable itemTable)
    {
        Dictionary<string, ScrollListItemView> newScrollList = new Dictionary<string, ScrollListItemView>();
        foreach (string itemId in itemTable.Keys)
        {
            if (m_CurrentScrollList.ContainsKey(itemId))
            {
                newScrollList[itemId] = m_CurrentScrollList[itemId];
                newScrollList[itemId].UpdateItem(itemTable[itemId]);
                m_CurrentScrollList.Remove(itemId);
            }
            else
            {
                ScrollListItemView itemInstance = Instantiate(m_ScrollListItemPrefab);
                itemInstance.transform.SetParent(transform);
                itemInstance.transform.localPosition = Vector3.zero;
                itemInstance.transform.localScale = Vector3.one;
                itemInstance.InitItemView(itemId, this);
                itemInstance.UpdateItem(itemTable[itemId]);
                newScrollList[itemId] = itemInstance;
            }
        }

        foreach (RoomView rV in m_CurrentScrollList.Values)
        {
            Destroy(rV.gameObject);
        }
        m_CurrentScrollList.Clear();

        m_CurrentScrollList = newScrollList;
    }

	public void SelectItemInList(string name, bool isSelected)
    {
        if (isSelected)
        {
            m_SelectedRoom = name;
        }
        else
        {
            if (!string.IsNullOrEmpty(m_SelectedRoom) && m_SelectedRoom.Equals(name))
            {
                m_SelectedRoom = null;
            }
        }
        onSelectedRoomChange.Invoke(m_SelectedRoom);
    }
}
