namespace LoupsGarous
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnusedIdentityUpdateOperationsDropdownView : IdentityUpdateOperationsDropdownView
    {
        protected enum Operation { RevealIdentity = 1 };

        protected override void InitOperationsDropdown()
        {
            base.InitOperationsDropdown();

            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            foreach (Operation op in Enum.GetValues(typeof(Operation)).Cast<Operation>())
            {
                Dropdown.OptionData option = new Dropdown.OptionData();
                switch (op)
                {
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
                case Operation.RevealIdentity:
                    m_GameLogic.BroadcastUnusedIdentity(m_Identity.Number);
                    break;
            }

            ResetDropdownValue();
        }
    }
}