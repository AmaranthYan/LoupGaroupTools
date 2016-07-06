namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class GameSessionService
    {
        public const int ALLOCATED_PHOTON_VIEW_IDS_NUMBER = 5;

        private static GameObject m_GameSession = null;
        public static GameObject GameSession { get { return m_GameSession; } }

        private static List<int> m_AllocatedPhotonViewIds = new List<int>();

        public static int[] AllocatePhotonViewIds()
        {
            int[] allocatedIds = new int[ALLOCATED_PHOTON_VIEW_IDS_NUMBER];
            for (int i = 0; i < ALLOCATED_PHOTON_VIEW_IDS_NUMBER; i++)
            {
                allocatedIds[i] = PhotonNetwork.AllocateViewID();
            }
            m_AllocatedPhotonViewIds.AddRange(allocatedIds);
            return allocatedIds;
        }

        private static void UnallocatePhotonViewIds()
        {
            foreach (int allocatedId in m_AllocatedPhotonViewIds)
            {
                PhotonNetwork.UnAllocateViewID(allocatedId);
            }
            m_AllocatedPhotonViewIds.Clear();
        }

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

        public static bool StartGameSession(int GameSessionPhotonViewId, int[] otherPhotonViewIds, PhotonPlayer[] players, GameModeModel gameMode, Dictionary<int, int> characterSet)
        {
            if (m_GameSession)
            {
                Debug.LogWarning("当前游戏进程不为空，请先结束当前进程。");
                return false;
            }

            m_GameSession = UnityEngine.Object.Instantiate(gameMode.GameSessionPrefab);

            PhotonView photonView = m_GameSession.GetComponent<PhotonView>();
            photonView.viewID = GameSessionPhotonViewId;

            GameLogicBase gameLogic = m_GameSession.GetComponent<GameLogicBase>();
            if (!gameLogic)
            {
                Debug.LogError("无法生成有效的游戏逻辑，游戏未能成功创建！");
                EndGameSession();
                return false;
            }
            gameLogic.InitGameLogic(otherPhotonViewIds, players, gameMode, characterSet);
            return true;
        }

        public static void EndGameSession()
        {
            if (m_GameSession)
            {
                UnityEngine.Object.DestroyImmediate(m_GameSession);
                UnallocatePhotonViewIds();
            }
            else
            {
                Debug.LogWarning("当前游戏进程为空。");
            }
            m_GameSession = null;
        }
    }
}
