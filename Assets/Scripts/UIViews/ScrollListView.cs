using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

[RequireComponent(typeof(ToggleGroup))]
public abstract class ScrollListView<T> : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_ScrollListItemPrefab = null;
    [SerializeField]
    protected ToggleGroup m_DefaultToggleGroup = null;
    [SerializeField]
    protected bool m_AllowMultiSelect = false;

    protected Dictionary<string, ScrollListItemView<T>> m_CurrentScrollList = new Dictionary<string, ScrollListItemView<T>>();
    protected string m_SelectedItemId = null;
    protected HashSet<string> m_MultiSelectedItemIds = new HashSet<string>();

    public UnityEvent<T> onSelectedItemChange = null;
    public UnityEvent<List<T>> onMultiSelectedItemsChange = null;

    protected abstract void Awake();

    public virtual void UpdateList(OrderedDictionary itemDictionary)
    {
        List<string> keys = m_CurrentScrollList.Keys.ToList();
        foreach (string itemId in keys)
        {
            if (!itemDictionary.Contains(itemId))
            {
                DeleteItemFromList(itemId);
            }
        }
        
        foreach (string itemId in itemDictionary.Keys)
        {
            if (m_CurrentScrollList.ContainsKey(itemId))
            {
                UpdateItemView(m_CurrentScrollList[itemId], itemDictionary[itemId]);
            }
            else
            {
                AddItemToList(itemId, itemDictionary[itemId]);               
            }
        }
    }

    protected virtual ScrollListItemView<T> InstantiateItemView()
    {
        ScrollListItemView<T> viewInstance = Instantiate(m_ScrollListItemPrefab).GetComponent<ScrollListItemView<T>>();
        viewInstance.transform.SetParent(transform);
        viewInstance.transform.localPosition = Vector3.zero;
        viewInstance.transform.localScale = Vector3.one;
        return viewInstance;
    }

    protected virtual void UpdateItemView(ScrollListItemView<T> itemView, object value)
    {
        itemView.UpdateItem(value);
    }

    public virtual void AddItemToList(string idInList, object value)
    {
        if (m_CurrentScrollList.ContainsKey(idInList)) { return; }

        m_CurrentScrollList[idInList] = InstantiateItemView();

        ToggleGroup toggleGroup = m_AllowMultiSelect ? null : m_DefaultToggleGroup;
        m_CurrentScrollList[idInList].InitItemView(idInList, toggleGroup, this);

        UpdateItemView(m_CurrentScrollList[idInList], value);
    }

    public virtual void UpdateItemInList(string idInList, T value)
    {
        if (!m_CurrentScrollList.ContainsKey(idInList)) { return; }

        UpdateItemView(m_CurrentScrollList[idInList], value);
    }

    public virtual void DeleteItemFromList(string idInList)
    {
        if (!m_CurrentScrollList.ContainsKey(idInList)) { return; }

        Destroy(m_CurrentScrollList[idInList].gameObject);
        m_CurrentScrollList.Remove(idInList);
    }

    public void SelectItemInList(string idInList, bool isSelected)
    {
        bool isChanged = false;

        if (!m_AllowMultiSelect)
        {
            if (isSelected)
            {
                m_SelectedItemId = idInList;
                isChanged = true;
            }
            else
            {
                if ((m_SelectedItemId != null) && m_SelectedItemId.Equals(idInList))
                {
                    m_SelectedItemId = null;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                onSelectedItemChange.Invoke((m_SelectedItemId != null) && m_CurrentScrollList.ContainsKey(m_SelectedItemId) ? m_CurrentScrollList[m_SelectedItemId].Item : default(T));
            }
        }
        else
        {
            if (isSelected)
            {
                isChanged = m_MultiSelectedItemIds.Add(idInList);
            }
            else
            {
                isChanged = m_MultiSelectedItemIds.Remove(idInList);
            }

            if (isChanged)
            {
                List<T> itemViews = m_MultiSelectedItemIds.Count() > 0 ? new List<T>() : null;
                foreach (string itemId in m_MultiSelectedItemIds)
                {
                    if (m_CurrentScrollList.ContainsKey(itemId))
                    {
                        itemViews.Add(m_CurrentScrollList[itemId].Item);
                    }
                }
                onMultiSelectedItemsChange.Invoke(itemViews);
            }            
        }
    }

    public void SelectAll(bool isSelected)
    {
        if (!m_AllowMultiSelect) { return; }

        foreach (string itemId in m_CurrentScrollList.Keys)
        {
            SelectItemInList(itemId, isSelected);
            m_CurrentScrollList[itemId].Toggle.isOn = isSelected;
        }
    }
}
