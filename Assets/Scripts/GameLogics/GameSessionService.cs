namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class GameSessionService
    {
        private static GameObject m_GameSession = null;
        public static GameObject GameSession { get { return m_GameSession; } }

        //返回GameConfig支持的最大玩家人数
        public static int SerializeGameConfigParameters(GameConfigView gameConfigView, out int gameModeId, out Dictionary<int, int> characterSet)
        {
            gameModeId = gameConfigView.CurrentGameMode.Id;
            List<DataPair<CharacterModel, CharacterSetting>> characters = gameConfigView.CurrentCharacters;
            int maxPlayersNumber = 0;
            characterSet = new Dictionary<int, int>();
            foreach (DataPair<CharacterModel, CharacterSetting> character in characters)
            {
                characterSet[character.Value1.Id] = character.Value2.Amount;
                maxPlayersNumber += (1 - character.Value1.CollateralCharacters) * character.Value2.Amount;
            }
            return maxPlayersNumber;
        }

        public static bool StartGameSession(PhotonPlayer[] players, GameModeModel gameMode, Dictionary<int, int> characterSet)
        {
            if (m_GameSession)
            {
                Debug.LogWarning("当前游戏进程不为空，请先结束当前进程。");
                return false;
            }

            m_GameSession = UnityEngine.Object.Instantiate(gameMode.GameSessionPrefab);
            GameLogicBase gameLogic = m_GameSession.GetComponent<GameLogicBase>();
            if (!gameLogic)
            {
                Debug.LogError("无法生成有效的游戏逻辑，游戏未能成功创建！");
                EndGameSession();
                return false;
            }
            gameLogic.InitGameLogic(players, characterSet);
            return true;
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
        }
    }
}
