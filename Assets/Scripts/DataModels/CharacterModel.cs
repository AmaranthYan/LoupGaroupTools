namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CharacterModel
    {
        [SerializeField]
        private int m_Id;
        [SerializeField]
        private int m_Camp;
        [SerializeField]
        private string m_DisplayName;
        [SerializeField]
        private string m_AmountRegex;
        [SerializeField]
        private int m_CollateralCharacters;
        //[SerializeField]
        //private Sprite m_Icon;
        [SerializeField]
        private Sprite m_Image;

        public int Id { get { return m_Id; } }
        public int Camp { get { return m_Camp; } }
        public string DisplayName { get { return m_DisplayName; } }
        public string AmountRegex { get { return m_AmountRegex; } }
        public int CollateralCharacters { get { return m_CollateralCharacters; } }
        //public Sprite Icon { get { return m_Icon; } }
        public Sprite Image { get { return m_Image; } }
    }
}
