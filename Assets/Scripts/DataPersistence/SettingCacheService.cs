namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    [XmlRoot("SettingCache")]
    public class SettingCacheService
    {
        [XmlIgnore]
        public const string CACHE_FILE_NAME = "DefaultSettings.cache";

        private List<CharacterSetting> m_CharacterSettings = new List<CharacterSetting>();
        [XmlArray("DefaultCharacterSettings")]
        [XmlArrayItem("CharacterSetting")]
        public List<CharacterSetting> CharacterSettings { get { return m_CharacterSettings; } set { m_CharacterSettings = value; } }

        private static SettingCacheService m_SettingCacheServiceInstance = null;
        public static SettingCacheService Instance
        {
            get
            {
                if (m_SettingCacheServiceInstance == null)
                {
                    m_SettingCacheServiceInstance = LoadCache();
                }
                return m_SettingCacheServiceInstance;
            }
        }

        private SettingCacheService() { }

        private static SettingCacheService LoadCache()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingCacheService));
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, CACHE_FILE_NAME), FileMode.OpenOrCreate))
                {                
                    return serializer.Deserialize(stream) as SettingCacheService;                
                }
            }
            catch (Exception e)
            {
                return new SettingCacheService();
            }
        }

        private void SaveCache()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingCacheService));
            try { 
                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, CACHE_FILE_NAME), FileMode.Create))
                {
                    serializer.Serialize(stream, this);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public CharacterSetting LoadCharacterSettingFromCache(int id)
        {           
            CharacterSetting setting = CharacterSettings.Find(cs => cs.Id == id);
            return setting != default(CharacterSetting) ? setting : null;
        }

        public void SaveCharacterSettingToCache(CharacterSetting setting)
        {
            if (setting == null) { return; }

            CharacterSettings.RemoveAll(cs => cs.Id == setting.Id);
            CharacterSettings.Add(setting);

            SaveCache();
        }
    }
}
