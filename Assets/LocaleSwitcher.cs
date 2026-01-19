using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocaleSwitcher : MonoBehaviour
{
    public void SetLocale(string localeCode)
    {
        // "be", "en", "ru"
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning("Locale not found: " + localeCode);
        }
    }
}
