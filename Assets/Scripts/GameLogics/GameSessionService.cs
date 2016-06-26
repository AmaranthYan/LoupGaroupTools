namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class GameSessionService
    {
        private static GameObject m_GameSession = null;
        public static GameObject GameSession { get { return m_GameSession; } }

        public static void StartGameSession(GameConfigView gameConfig)
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;
            GameModeModel gameMode = gameConfig.CurrentGameMode;
            List<DataPair<CharacterModel, CharacterSetting>> characters = gameConfig.CurrentCharacters;

            int minPlayersNumber = 0;
            foreach(DataPair<CharacterModel, CharacterSetting> character in characters)
            {
                minPlayersNumber += 1 + character.Value1.CollateralCharacters;
            }
            if (players.Length <= minPlayersNumber)
            {
                PhotonNetwork.room.open = false;
                if (m_GameSession)
                {
                    UnityEngine.Object.Destroy(m_GameSession);
                }
                m_GameSession = UnityEngine.Object.Instantiate(gameMode.GameSessionPrefab);
                //m_GameSession.GetComponent<GameLogicBase>();

            }            
        }

        public static void EndGameSession()
        {
            if (!m_GameSession)
            {
                UnityEngine.Object.Destroy(m_GameSession);
            }
            m_GameSession = null;
            PhotonNetwork.room.open = true;
        }
    }
}
