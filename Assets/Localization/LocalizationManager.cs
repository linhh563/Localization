using System.Collections.Generic;
using UnityEngine;


namespace Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance;

        private Dictionary<string, Dictionary<string, string>> _localizedData;
        private string _currentLanguage;

        public static Event OnLanguageChanged;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            DontDestroyOnLoad(this);
        }


        public void LoadLocalizationData()
        {

        }
    }
}
