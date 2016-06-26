namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class GameConfigView : MonoBehaviour
    {
        [SerializeField]
        private GameModeDatabase m_GameModeDatabase = null;
        [SerializeField]
        private CharacterDatabase m_CharacterDatabase = null;

        //[SerializeField]
        //private Dropdown m_GameModeDropdown = null;

        public UnityTypedEvent.StringListEvent onGameModeInit = new UnityTypedEvent.StringListEvent();
        public UnityTypedEvent.BoolEvent onGameModeValidate = new UnityTypedEvent.BoolEvent();
        public UnityTypedEvent.HashtableEvent onCharacterListUpdate = new UnityTypedEvent.HashtableEvent();

        private GameModeModel m_CurrentGameMode = null;
        public GameModeModel CurrentGameMode { get { return m_CurrentGameMode; } }
        private List<DataPair<CharacterModel, CharacterSetting>> m_CurrentCharacters = null;
        public List<DataPair<CharacterModel, CharacterSetting>> CurrentCharacters { get { return m_CurrentCharacters; } }

        void Start()
        {
            if (!m_GameModeDatabase)
            {
                Debug.LogError("游戏模式库未加载！");
                return;
            }
            else if (m_GameModeDatabase.GameModeModels.Length == 0)
            {
                Debug.LogError("游戏模式库为空！");
                return;
            }
            if (!m_CharacterDatabase)
            {
                Debug.LogError("游戏角色库未加载！");
                return;
            }
            else if (m_CharacterDatabase.CharacterModels.Length == 0)
            {
                Debug.LogError("游戏角色库为空！");
                return;
            }

            GameModeModel[] availableGameModes = m_GameModeDatabase.GameModeModels;
            List<string> displayNames = new List<string>();
            foreach (GameModeModel gameMode in availableGameModes)
            {
                displayNames.Add(gameMode.DisplayName);
            }
            onGameModeInit.Invoke(displayNames);

            SelectGameMode(0);
        }

        public void SelectGameMode(int modeIndex)
        {
            if (!m_GameModeDatabase || m_GameModeDatabase.GameModeModels.Length <= modeIndex) { return; }
            m_CurrentGameMode = m_GameModeDatabase.GameModeModels[modeIndex];
            ValidateGameMode(m_CurrentGameMode);
            UpdateCharacterList(m_CurrentGameMode);
        }

        public void UpdateCharacterList(GameModeModel gameMode)
        {
            if (!m_CharacterDatabase) { return; }
            Hashtable hashtable = new Hashtable();
            CharacterModel[] enabledCharacters = m_CharacterDatabase.CharacterModels.Where(c => !gameMode.DisabledCharacters.Contains(c.Id)).ToArray();
            foreach (CharacterModel character in enabledCharacters)
            {
                hashtable.Add(character.Id.ToString(), character);
            }
            onCharacterListUpdate.Invoke(hashtable);
        }

        public void ValidateGameMode(GameModeModel gameMode)
        {
            onGameModeValidate.Invoke(gameMode.GameSessionPrefab != null);
        }

        public void SelectCharacters(List<DataPair<CharacterModel, CharacterSetting>> characters)
        {
            m_CurrentCharacters = characters;
        }
    }
}
