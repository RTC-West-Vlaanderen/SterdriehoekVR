using System;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    private bool _isFrench = false;

    public bool IsFrench
    {
        get
        {
            return _isFrench;
        }
        set
        {
            if (value != _isFrench)
            {
                _isFrench = value;
            }
        }
    }
    public static LanguageManager LanguageManagerSingleton;
    private bool _hasUserPickLanguage;

    public bool HasUserPickLanguage
    {
        get
        {
            return _hasUserPickLanguage;
        }
        set
        {
            if (value != _hasUserPickLanguage) _hasUserPickLanguage = value;    
        }
    }
    /// <summary>
    /// This method sets the language of the app, if it's french, the bool changes the language of the whole app
    /// </summary>
    /// <param name="isFrench"></param>
    public void LanguageButton(bool isFrench)
    {
        _isFrench = isFrench;
        _hasUserPickLanguage = true;
    }
    

    private void Start()
    {
        LanguageManagerSingleton = this;
    }
}
