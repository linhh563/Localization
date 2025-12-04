using TMPro;
using UnityEngine;


namespace Localization
{
    /// <summary>
    /// The component wad assigned to Text Mesh Pro game object for manage the localized text.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        public TextAsset dataFile;
        public string textKey;

        private TMP_Text _text;


        void OnEnable()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }
        }
    }
}
