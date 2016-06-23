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
        private MonoScript m_GameLogicScript;

        public int Id { get { return m_Id; } }
        public string DisplayName { get { return m_DisplayName; } }
        public int[] DisabledCharacters { get { return m_DisabledCharacters; } }
        public Type GameLogicScriptType { get { return m_GameLogicScript.GetClass(); } }
    }
}
