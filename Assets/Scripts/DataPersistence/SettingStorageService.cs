namespace LoupsGarous
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    [XmlRoot("LocalSettings")]
    public class SettingStorageService
    {
        [XmlIgnore]
        public const string STORAGE_FILE_NAME = "LocalSettings.set";

        private List<CharacterSetting> m_CharacterSettings = new List<CharacterSetting>();
        [XmlArray("LocalCharacterSettings")]
        [XmlArrayItem("CharacterSetting")]
        public List<CharacterSetting> CharacterSettings { get { return m_CharacterSettings; } set { m_CharacterSettings = value; } }

        private static SettingStorageService m_SettingStorageServiceInstance = null;
        public static SettingStorageService Instance
        {
            get
            {
                if (m_SettingStorageServiceInstance == null)
                {
                    m_SettingStorageServiceInstance = LoadSettings();
                }
                return m_SettingStorageServiceInstance;
            }
        }

        private SettingStorageService() { }

        private static SettingStorageService LoadSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingStorageService));
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, STORAGE_FILE_NAME), FileMode.OpenOrCreate))
                {                
                    return serializer.Deserialize(stream) as SettingStorageService;                
                }
            }
            catch (Exception e)
            {
                Debug.LogError("设置读取失败！");
                return new SettingStorageService();
            }
        }

        private void SaveSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingStorageService));
            try { 
                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, STORAGE_FILE_NAME), FileMode.Create))
                {
                    serializer.Serialize(stream, this);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("设置保存失败！");
            }
        }

        public CharacterSetting RetrieveCharacterSetting(int id)
        {           
            CharacterSetting setting = CharacterSettings.Find(cs => cs.Id == id);
            return setting != default(CharacterSetting) ? setting : null;
        }

        public void StoreCharacterSetting(CharacterSetting setting)
        {
            if (setting == null) { return; }

            CharacterSettings.RemoveAll(cs => cs.Id == setting.Id);
            CharacterSettings.Add(setting);

            SaveSettings();
        }
    }
}
