namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [RequireComponent(typeof(Dropdown))]
    public class CharactersDropdownView : MonoBehaviour
    {
        [SerializeField]
        private Dropdown m_CharactersDropdown = null;
        [SerializeField]
        private CharacterDatabase m_CharacterDatabase = null;
        [SerializeField]
        private string m_UnknownCharacterText = string.Empty;

        private GameLogicBase m_GameLogic = null;

        void Start()
        {
            m_GameLogic = FindObjectOfType<GameLogicBase>();
            m_CharactersDropdown = this.GetComponent<Dropdown>();

            PopulateCharactersDropdown();
        }

        private void PopulateCharactersDropdown()
        {
            int[] characterIds = m_GameLogic.CharacterIds;

            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData(m_UnknownCharacterText));
            foreach (int id in characterIds)
            {
                Dropdown.OptionData option = new Dropdown.OptionData(m_CharacterDatabase.CharacterModels[id].DisplayName, m_CharacterDatabase.CharacterModels[id].Image);
                optionList.Add(option);
            }

            m_CharactersDropdown.options = optionList;
        }
    }
}
