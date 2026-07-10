using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private List<string> _keys = new List<string>();
    private bool _loaded;

    public override void OnInspectorGUI()
    {
        LocalizedText localizedText = (LocalizedText)target;

        if (!_loaded)
        {
            LoadKeysFromJson();
            _loaded = true;
        }

        if (_keys.Count == 0)
        {
            EditorGUILayout.HelpBox("Geen keys gevonden...", MessageType.Warning);
            localizedText.key = EditorGUILayout.TextField("Key", localizedText.key);
            return;
        }

        int currentIndex = _keys.IndexOf(localizedText.key);

        if (currentIndex < 0)
        {
            EditorGUILayout.HelpBox(
                $"Key '{localizedText.key}' komt niet exact overeen met een key in de JSON!",
                MessageType.Error);
            // Toon het huidige (foutieve) key-veld apart, overschrijf NIETS automatisch
            localizedText.key = EditorGUILayout.TextField("Huidige key (fout)", localizedText.key);
        }

        int newIndex = EditorGUILayout.Popup("Key", Mathf.Max(currentIndex, 0), _keys.ToArray());

        if (currentIndex >= 0 && newIndex != currentIndex)
        {
            Undo.RecordObject(localizedText, "Change Localization Key");
            localizedText.key = _keys[newIndex];
            EditorUtility.SetDirty(localizedText);
        }

        if (GUILayout.Button("Keys opnieuw laden uit JSON"))
            _loaded = false;
    }

    private void LoadKeysFromJson()
    {
        _keys.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, "languageData.json");

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<LanguageManager.LanguageDataWrapper>(json);

        if (wrapper?.entries == null)
            return;

        foreach (var entry in wrapper.entries)
        {
            _keys.Add(entry.key);
        }
    }
}