using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ScrollListItemView<T> : MonoBehaviour
{
    [SerializeField]
    protected Toggle m_Toggle = null;

    protected string m_IdInList = null;
    public string IdInList { get { return m_IdInList; } set { m_IdInList = value; } }
    protected T m_Item = default(T);
    public T Item { get { return m_Item; } set { m_Item = value; } }

    protected ScrollListView<T> m_ScrollListView = null; 
    public ScrollListView<T> ScrollListView { get { return m_ScrollListView; } set { m_ScrollListView = value; } }

    protected void OnDestroy()
    {
        IsSelectedInList(false);
    }

    public virtual void InitItemView(string idInList, ToggleGroup toggleGroup, ScrollListView<T> scrollListView)
    {
        m_IdInList = idInList;
        m_ScrollListView = scrollListView;

        if (m_Toggle)
        {
            m_Toggle.group = toggleGroup;
        }
    }

    public virtual void UpdateItem(object value) { }    

    public virtual void IsSelectedInList(bool isSelected)
    {
        if (m_ScrollListView)
        {
            m_ScrollListView.SelectItemInList(m_IdInList, isSelected);
        }
    }
}
