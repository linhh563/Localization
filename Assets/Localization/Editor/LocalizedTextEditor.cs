using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Localization
{
    [CustomEditor(typeof(LocalizedText))]
    public class LocalizedTextEditor : Editor
    {
        /// <summary>
        /// The text key get from csv file.
        /// </summary>
        private string[] _keys;

        /// <summary>
        /// The selected key's index in key list.
        /// </summary>
        private int _selectedIndex;


        public override void OnInspectorGUI()
        {
            // Get target object
            LocalizedText script = (LocalizedText)target;

            // Draw csvFile normally
            script.dataFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", script.dataFile, typeof(TextAsset), false);

            // If CSV assigned, parse first row
            if (script.dataFile != null)
            {
                ParseCsv(script.dataFile.text);

                // Draw dropdown
                if (_keys != null && _keys.Length > 0)
                {
                    _selectedIndex = Mathf.Max(0, System.Array.IndexOf(_keys, script.textKey));

                    _selectedIndex = EditorGUILayout.Popup("Key", _selectedIndex, _keys);
                    script.textKey = _keys[_selectedIndex];
                }
            }

            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
            }
        }


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
