namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections;
    using System.Linq;

    public class PollResultView : ScrollListItemView<DataPair<PlayerIdentity, List<PlayerIdentity>>>
    {
        [Header("Events")]
        public UnityTypedEvent.PlayerIdentityEvent onCandidateUpdate = new UnityTypedEvent.PlayerIdentityEvent();
        public UnityEvent onAbstain = new UnityEvent();
        public UnityTypedEvent.OrderedDictionaryEvent onVotersUpdate = new UnityTypedEvent.OrderedDictionaryEvent();
        
        public override void UpdateItem(object value)
        {
            base.UpdateItem(value);
            m_Item = (DataPair<PlayerIdentity, List<PlayerIdentity>>)value;

            if (m_Item.Value1 != null)
            {
                onCandidateUpdate.Invoke(m_Item.Value1);
            }
            else
            {
                onAbstain.Invoke();
            }

            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (PlayerIdentity voter in m_Item.Value2)
            {
                dictionary.Add(voter.Number.ToString(), voter);
            }
            onVotersUpdate.Invoke(dictionary);            
        }
    }
}