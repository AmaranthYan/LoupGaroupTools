namespace LoupsGarous
{
    using UnityEngine;

    public class GameSessionServiceView : MonoBehaviour
    {
        [SerializeField]
        private GameConfigView m_GameConfigView = null;

        public void StartGameSession()
        {
            GameSessionService.StartGameSession(m_GameConfigView);
            GameSessionService.GameSession.transform.parent = this.transform;
        }
    }
}
