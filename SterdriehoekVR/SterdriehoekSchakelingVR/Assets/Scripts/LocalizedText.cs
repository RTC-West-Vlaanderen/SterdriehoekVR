using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [Tooltip("Moet overeenkomen met een 'key' in languageData.json")]
    public string key;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        if (_text == null)
        {
            Debug.LogError($"[LocalizedText] Geen TMP_Text component gevonden op {gameObject.name}", this);
        }
    }

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;

        if (LanguageManager.LanguageManagerSingleton != null)
            UpdateText();
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (string.IsNullOrEmpty(key) || _text == null) return;

        _text.text = LanguageManager.LanguageManagerSingleton.GetTranslation(key);
    }
}