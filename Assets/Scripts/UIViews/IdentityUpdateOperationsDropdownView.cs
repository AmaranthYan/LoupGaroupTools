namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class IdentityUpdateOperationsDropdownView : MonoBehaviour
    {
        private enum Operation { };

        [SerializeField]
        protected Dropdown m_OperationsDropdown = null;

        protected GameLogicBase m_GameLogic = null;
        protected PlayerIdentity m_Identity = null;

        void Start()
        {
            m_GameLogic = FindObjectOfType<GameLogicBase>();

            InitOperationsDropdown();
        }

        public void RetrieveIdentity(PlayerIdentity identity)
        {
            m_Identity = identity;
        }

        protected abstract void InitOperationsDropdown();
        public abstract void ExecuteOperation(int opId);
    }
}