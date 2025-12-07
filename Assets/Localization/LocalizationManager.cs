using System.Collections.Generic;
using System.Text;
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
            var localizedTables = Resources.LoadAll<TextAsset>("Localization/");

            foreach (var table in localizedTables)
            {
                LoadLocalizedFile(table);
            }
        }


        /// <summary>
        /// Load localization in passed file.
        /// </summary>
        private void LoadLocalizedFile(TextAsset csvFile)
        {
            Debug.Log(csvFile.name);

            _localizedData = new Dictionary<string, Dictionary<string, string>>();

            // Use the new parse method for header row
            // Break the CSV into lines using any newline format, use RemoveEmptyEntries to ignore the blank lines
            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return;

            List<string> headers = ParseCsvLine(lines[0]);
            for (int i = 1; i < headers.Count; i++)
            {
                _localizedData[headers[i].Trim()] = new Dictionary<string, string>();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                // Use the new parser method for each data row
                List<string> values = ParseCsvLine(lines[i]);
                string key = values[0].Trim();

                for (int j = 1; j < values.Count && j < headers.Count; j++)
                {
                    string language = headers[j].Trim();
                    string value = values[j].Trim(); // The parser handles quotes, so we can still trim.
                    _localizedData[language][key] = value;
                }
            }
        }


        /// <summary>
        /// Handle the issues that Split(',') cannot.
        /// </summary>
        // E.g. commas in quotes, escaped quotes (""), handles empty fields, ignore comma separators inside quotes, ...
        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    // If we are in quotes, check for a closing quote
                    if (c == '"')
                    {
                        // Check if it's an escaped quote ("")
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++; // Skip the next quote
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else
                {
                    // If we are not in quotes
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        fields.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
            }

            fields.Add(currentField.ToString());
            return fields;
        }


        public string GetLocalizedValue(string table, string key)
        {

            if (_localizedData.ContainsKey(_currentLanguage) && _localizedData[_currentLanguage].ContainsKey(key))
            {
                return _localizedData[_currentLanguage][key];
            }

            Debug.LogWarning($"Localization key '{key}' not found for language '{_currentLanguage}'.");
            return key;
        }
    }
}
