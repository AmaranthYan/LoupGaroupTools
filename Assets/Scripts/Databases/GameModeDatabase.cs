namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections;

    [CreateAssetMenu(fileName = "GameModeDatabase", menuName = "LoupsGarous/GameModeDatabase", order = 1)]
    public class GameModeDatabase : ScriptableObject
    {
        [SerializeField]
        private GameModeModel[] m_GameModeModels;
        public GameModeModel[] GameModeModels { get { return m_GameModeModels; } }
    }
}