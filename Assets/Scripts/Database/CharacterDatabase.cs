namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections;

    [CreateAssetMenu(fileName = "LoupsGarousCharacterDatabase", menuName = "LoupsGarous/CharacterDatabase", order = 1)]
    public class CharacterDatabase : ScriptableObject
    {
        [SerializeField]
        private CharacterModel[] m_CharacterModels;
        public CharacterModel[] CharacterModels { get { return m_CharacterModels; } }
    }
}