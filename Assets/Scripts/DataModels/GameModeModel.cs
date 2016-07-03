namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GameModeModel
    {
        [SerializeField]
        private int m_Id;
        [SerializeField]
        private string m_DisplayName;
        [SerializeField]
        private int[] m_DisabledCharacters;
        [SerializeField]
        [Multiline]
        private string m_Description;
        [SerializeField]
        private GameObject m_GameSessionPrefab;

        public int Id { get { return m_Id; } }
        public string DisplayName { get { return m_DisplayName; } }
        public int[] DisabledCharacters { get { return m_DisabledCharacters; } }
        public string Description { get { return m_Description; } }
        public GameObject GameSessionPrefab { get { return m_GameSessionPrefab; } }
    }
}
