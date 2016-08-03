namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PlayerIdentityUpdateOperationsDropdownView : IdentityUpdateOperationsDropdownView
    {
        protected enum Operation { MarkAsDead = 1, MarkAsCaptain, RevealIdentity };

        protected override void InitOperationsDropdown()
        {
            base.InitOperationsDropdown();

            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            foreach (Operation op in Enum.GetValues(typeof(Operation)).Cast<Operation>())
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                switch (op)
                {
                    case Operation.MarkAsDead:
                        option.text = "标记死亡";
                        break;
                    case Operation.MarkAsCaptain:
                        option.text = "标记警长";
                        break;
                    case Operation.RevealIdentity:
                        option.text = "揭示身份";
                        break;
                }
                optionList.Add(option);
            }
            
            m_OperationsDropdown.AddOptions(optionList);
            ResetDropdownValue();
        }

        public override void ExecuteOperation(int opId)
        {
            if (m_Identity == null) { return; }

            Operation op = (Operation)opId;
            switch (op)
            {
                case Operation.MarkAsDead:
                    m_GameLogic.ReversePlayerSurvivalStatus(m_Identity.Number);
                    break;
                case Operation.MarkAsCaptain:
                    m_GameLogic.PromotePlayerToCaptain(m_Identity.Number);
                    break;
                case Operation.RevealIdentity:
                    m_GameLogic.BroadcastPlayerIdentity(m_Identity.Number);
                    break;
            }

            ResetDropdownValue();
        }
    }
}