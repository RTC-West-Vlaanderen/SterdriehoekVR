using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager LanguageManagerSingleton;
    public static event Action OnLanguageChanged;

    private bool _isFrench = false;
    private bool _hasUserPickLanguage;
    
    public bool IsFrench
    {
        get => _isFrench;
        set
        {
            if (value != _isFrench)
                _isFrench = value;
        }
    }

    public bool HasUserPickLanguage
    {
        get => _hasUserPickLanguage;
        set
        {
            if (value != _hasUserPickLanguage)
                _hasUserPickLanguage = value;
        }
    }

    [Serializable]
    public class LanguageEntry
    {
        public string key;
        public string nl;
        public string fr;
    }

    [Serializable]
    public class LanguageDataWrapper
    {
        public List<LanguageEntry> entries = new List<LanguageEntry>();
    }

    
    private LanguageDataWrapper _languageData;
    private Dictionary<string, LanguageEntry> _lookup = new Dictionary<string, LanguageEntry>();

    private const string FileName = "languageData.json";
    private string PersistentPath => Path.Combine(Application.persistentDataPath, FileName);
    private string StreamingPath => Path.Combine(Application.streamingAssetsPath, FileName);

    [SerializeField] private List<SpriteRenderer> _spriteRendererSceneNewColors;
    [SerializeField] private Sprite _SpriteSterNewcolors;
    [SerializeField] private Sprite _SpriteSterNewcolorsFR;
    
    [SerializeField] private List<SpriteRenderer> _spriteRendererSceneNewColorsWithCoding;
    [SerializeField] private Sprite _SpritespriteRendererSceneNewColorsWithCoding;
    [SerializeField] private Sprite _SpritespriteRendererSceneNewColorsWithCodingFR;
    
    [SerializeField] private List<SpriteRenderer> _spriteRendererSceneNDriehoek;
    [SerializeField] private Sprite _SpriteDriehoekNewcolors;
    [SerializeField] private Sprite _SpriteDriehoekNewcolorsFR;

    
    private void Start()
    {
        LanguageManagerSingleton = this;
        StartCoroutine(InitLanguageFile());
    }

    private IEnumerator InitLanguageFile()
    {
        // Tijdens ontwikkeling altijd verversen zodat wijzigingen in StreamingAssets
        // meteen zichtbaar zijn. Voor productie kan je dit terugzetten naar
        // "if (!File.Exists(PersistentPath))" zodat spelers hun eigen aanpassingen behouden.
        yield return CopyStreamingAssetToPersistent();

        ReadJson();
    }

    private IEnumerator CopyStreamingAssetToPersistent()
    {
        // Op Android (dus ook Pico) zit StreamingAssets in een .apk/.jar,
        // dus moet je UnityWebRequest gebruiken om het uit te lezen.
        using (UnityWebRequest request = UnityWebRequest.Get(StreamingPath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(PersistentPath, request.downloadHandler.data);
                Debug.Log($"[LanguageManager] Bestand gekopieerd naar: {PersistentPath}");
            }
            else
            {
                Debug.LogError($"[LanguageManager] Kon StreamingAssets bestand niet lezen: {request.error}");
            }
        }
    }

    private void ReadJson()
    {
        try
        {
            string json = File.ReadAllText(PersistentPath);
            _languageData = JsonUtility.FromJson<LanguageDataWrapper>(json);

            _lookup.Clear();
            foreach (var entry in _languageData.entries)
            {
                _lookup[entry.key] = entry;
            }

            Debug.Log($"[LanguageManager] {_languageData.entries.Count} vertalingen geladen.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[LanguageManager] Fout bij lezen JSON: {e.Message}");
        }
    }

    public string GetTranslation(string key)
    {
        if (!_lookup.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"[LanguageManager] Geen vertaling gevonden voor key '{key}'");
            return key;
        }

        return IsFrench ? entry.fr : entry.nl;
    }

    public void LanguageButton(bool isFrench)
    {
        _isFrench = isFrench;
        _hasUserPickLanguage = true;
        OnLanguageChanged?.Invoke(); // <-- dit triggert alle teksten om te updaten
        ChangeImagesByLanguage();
    }

    void ChangeImagesByLanguage()
    {
        Sprite sterSprite = _isFrench ? _SpriteSterNewcolorsFR : _SpriteSterNewcolors;
        ApplySpriteToList(_spriteRendererSceneNewColors, sterSprite);

        Sprite codingSprite = _isFrench 
            ? _SpritespriteRendererSceneNewColorsWithCodingFR 
            : _SpritespriteRendererSceneNewColorsWithCoding;
        ApplySpriteToList(_spriteRendererSceneNewColorsWithCoding, codingSprite);

        Sprite driehoekSprite = _isFrench ? _SpriteDriehoekNewcolorsFR : _SpriteDriehoekNewcolors;
        ApplySpriteToList(_spriteRendererSceneNDriehoek, driehoekSprite);
    }

    private void ApplySpriteToList(List<SpriteRenderer> renderers, Sprite sprite)
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
                renderer.sprite = sprite;
        }
    }
    /// <summary>
    /// Handig om vanop het toestel zelf (of via een debug-scherm) een nieuwe entry toe te voegen
    /// en meteen op te slaan naar persistentDataPath.
    /// </summary>
    public void AddOrUpdateEntry(string key, string nl, string fr)
    {
        var existing = _languageData.entries.Find(e => e.key == key);
        if (existing != null)
        {
            existing.nl = nl;
            existing.fr = fr;
        }
        else
        {
            _languageData.entries.Add(new LanguageEntry { key = key, nl = nl, fr = fr });
        }

        _lookup[key] = _languageData.entries.Find(e => e.key == key);
        SaveJson();
    }

    private void SaveJson()
    {
        string json = JsonUtility.ToJson(_languageData, true);
        File.WriteAllText(PersistentPath, json);
        Debug.Log($"[LanguageManager] JSON opgeslagen naar: {PersistentPath}");
    }
    
    
    
}