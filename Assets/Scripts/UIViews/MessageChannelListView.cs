using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using LoupsGarous;

public class MessageChannelListView : ScrollListView<MessageChannelModel>
{
    [Serializable]
    public class SingleItemEvent : UnityEvent<MessageChannelModel> { }
    [Serializable]
    public class MultiItemEvent : UnityEvent<List<MessageChannelModel>> { }

    [SerializeField]
    private Messager m_Messager = null;
    public Messager Messager { get { return m_Messager; } }

    public new SingleItemEvent onSelectedItemChange = new SingleItemEvent();
    public new MultiItemEvent onMultiSelectedItemsChange = new MultiItemEvent();

    protected override void Awake()
    {
        if (!m_Messager)
        {
            Debug.LogError("消息台不能为空！");
        }

        base.onSelectedItemChange = onSelectedItemChange;
        base.onMultiSelectedItemsChange = onMultiSelectedItemsChange;
    }

    public override void UpdateList(OrderedDictionary itemDictionary)
    {
        if (!m_Messager) { return; }

        base.UpdateList(itemDictionary);
    }

    public void AddChannel()
    {
        if (!m_Messager) { return; }

        m_Messager.AddChannel();
    }

    public void DeleteChannels()
    {
        if (!m_Messager) { return; }

        foreach (string itemId in m_MultiSelectedItemIds)
        {
            if (m_CurrentScrollList.ContainsKey(itemId))
            {
                m_Messager.DeleteChannel(m_CurrentScrollList[itemId].Item.ChannelId);
            }
        }        
    }
}