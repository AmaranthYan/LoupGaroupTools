namespace LoupsGarous
{
    using UnityEngine;
    using System;

    public class MyIdentityView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_MePlaceholder = null;
        
        public void UpdateTransform(RectTransform rectTransform)
        {
            rectTransform.SetParent(m_MePlaceholder);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}