namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

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

        protected void ResetDropdownValue()
        {
            m_OperationsDropdown.value = 0;
        }

        protected virtual void InitOperationsDropdown()
        {
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("放弃操作"));
            m_OperationsDropdown.options = optionList;
        }

        public abstract void ExecuteOperation(int opId);
    }
}