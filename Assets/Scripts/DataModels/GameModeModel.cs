namespace LoupsGarous
{
    using UnityEngine;
    using UnityEditor;
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
        private GameObject m_GameLogic;

        public int Id { get { return m_Id; } }
        public string DisplayName { get { return m_DisplayName; } }
        public int[] DisabledCharacters { get { return m_DisabledCharacters; } }
        public GameObject GameLogic { get { return m_GameLogic; } }
    }
}
