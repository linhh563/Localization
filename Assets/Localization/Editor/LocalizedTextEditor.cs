using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Localization
{
    [CustomEditor(typeof(LocalizedText))]
    public class LocalizedTextEditor : Editor
    {
        private static string _lastHash = null;

        private static List<string> _tables = new List<string>();   // Localization tables cache.
        private static string[] _tableArray;                        // Tables popup dropdown cache.
        private static TextAsset[] _localizedTables;                // Csv files cache.
        private int _tableSelectedIndex = 0;
        private string[] _keys;                                     // The text key get from csv file.
        private int _keySelectedIndex = 0;                          // The selected key's index in key list.


        void OnEnable()
        {
            RefreshIfNeeded();
        }


        public override void OnInspectorGUI()
        {
            RefreshIfNeeded();

            // Get target object
            LocalizedText script = (LocalizedText)target;

            // Create localization table dropdown
            if (_tableArray.Length > 0)
            {
                _tableSelectedIndex = Mathf.Max(0, _tables.IndexOf(script.localizationTable));

                // Create dropdown
                _tableSelectedIndex = EditorGUILayout.Popup("Localization Table", _tableSelectedIndex, _tableArray);
                script.localizationTable = _tables[_tableSelectedIndex];
            }

            // If CSV assigned, parse first row
            if (_tableSelectedIndex >= 0 && _tableSelectedIndex < _localizedTables.Length)
            {
                ParseCsv(_localizedTables[_tableSelectedIndex].text);

                // Draw dropdown
                if (_keys != null && _keys.Length > 0)
                {
                    _keySelectedIndex = Mathf.Max(0, System.Array.IndexOf(_keys, script.textKey));

                    _keySelectedIndex = EditorGUILayout.Popup("Key", _keySelectedIndex, _keys);
                    script.textKey = _keys[_keySelectedIndex];
                }
            }

            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
            }
        }


        /// <summary>
        /// Load localization files if had changed in Resources/Table.
        /// </summary>
        private void RefreshIfNeeded()
        {
#if UNITY_EDITOR
            // Find all TextAsset in Resources/Localization
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/Resources/Localization" });

            // Create a hash base on the file list
            string newHash = string.Join(",", guids);

            // If hashes is the same, do nothing
            if (newHash == _lastHash) return;

            // Update hash
            _lastHash = newHash;
            Debug.Log("Update hash.");
#endif

            LoadTables();
        }


        /// <summary>
        /// Load TextAsset files from Resources.
        /// </summary>
        // Only loads when the inspector is opened or Localization Resources changed, not continuously
        private void LoadTables()
        {
            _tables.Clear();

            // Load csv files from Resources
            _localizedTables = Resources.LoadAll<TextAsset>("Localization/");

            if (_tables == null) { _tables = new List<string>(); }

            foreach (var table in _localizedTables)
            {
                _tables.Add(table.name);
            }

            _tableArray = _tables.ToArray();
        }


        /// <summary>
        /// Get the keys in passed csv file.
        /// </summary>
        private void ParseCsv(string csv)
        {
            List<string> columnList = new List<string>();

            // Split into lines
            string[] lines = csv.Split('\n');

            // skip the header
            for (int i = 1; i < lines.Length; i++)
            {
                string rawLine = lines[i];
                string line = rawLine.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Split columns
                string[] cols = line.Split(',');

                if (cols.Length > 0)
                {
                    columnList.Add(cols[0].Trim());
                }
            }

            _keys = columnList.ToArray();
        }
    }
}