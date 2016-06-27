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
                    Debug.LogWarning("当前游戏进程不为空，请先结束当前进程。");
                    return;
                }
                m_GameSession = UnityEngine.Object.Instantiate(gameMode.GameSessionPrefab);

                GameLogicBase gameLogic = m_GameSession.GetComponent<GameLogicBase>();
                if (!gameLogic)
                {
                    Debug.LogError("无法生成有效的游戏逻辑，游戏未能成功创建！");
                    EndGameSession();
                    return;
                }
                Dictionary<int, int> characterSet = new Dictionary<int, int>();
                foreach (DataPair<CharacterModel, CharacterSetting> character in characters)
                {
                    characterSet[character.Value1.Id] = character.Value2.Amount;
                }
                gameLogic.InitGameLogic(players, characterSet);
            }            
        }

        public static void EndGameSession()
        {
            if (m_GameSession)
            {
                UnityEngine.Object.Destroy(m_GameSession);
            }
            else
            {
                Debug.LogWarning("当前游戏进程为空。");
            }
            m_GameSession = null;
            PhotonNetwork.room.open = true;
        }
    }
}
