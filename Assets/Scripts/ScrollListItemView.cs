using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ScrollListItemView : MonoBehaviour
{
    [SerializeField]
    protected Toggle m_Toggle = null;

    protected string m_IdInList = null;
    public string IdInList { get { return m_IdInList; } set { m_IdInList = value; } }
    protected ScrollListView m_ScrollListView = null;
    public ScrollListView ScrollListView { get { return m_ScrollListView; } set { m_ScrollListView = value; } }

    protected void OnDestroy()
    {
        IsSelectedInList(false);
    }

    public virtual void InitItemView(string idInList, ScrollListView scrollListView)
    {
        m_IdInList = idInList;
        m_ScrollListView = scrollListView;

        if (m_Toggle)
        {
            m_Toggle.group = m_ScrollListView.ToggleGroup;
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
