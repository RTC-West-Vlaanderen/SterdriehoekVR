using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    private bool _isFrench = false;
    /// <summary>
    /// This method sets the language of the app, if it's french, the bool changes the language of the whole app
    /// </summary>
    /// <param name="isFrench"></param>
    public void LanguageButton(bool isFrench)
    {
        _isFrench = isFrench;
    }
}
