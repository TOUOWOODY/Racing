using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageOption
{
    public static List<string> languageList;

    public static SystemLanguage currentLanguage;

    //public static SystemLanguage cLanguage;

    public const string languageKey = "SavedLanguage";

    static int originLanguageValue;
    
    public static void Initialize(SystemLanguage Lan)
    {
        //if (languageList != null)
        //{
        //    //return;
        //}
        
        currentLanguage = Lan;
        //currentLanguage = Application.systemLanguage;
        switch (currentLanguage)
        {
            case SystemLanguage.Korean:
                
                break;
            case SystemLanguage.English:
                
                break;
            default:
                currentLanguage = SystemLanguage.English;
                break;
        }

        languageList = new List<string>();

        originLanguageValue = PlayerPrefs.GetInt(languageKey);

        AddLanguageList();

        LoadLanguage();
    }

    private static void AddLanguageList()
    {
        foreach (var item in DesignConstStorage.LocalizeTable.ColumnHeader)
        {
            if (item.Key.ToString() != "key")
            {
                languageList.Add(item.Key.ToString());
            }
        }
    }

    public static void ReloadCheck()
    {
        if (originLanguageValue != PlayerPrefs.GetInt(languageKey))
        {
            Debug.Log("Reload Please.");
        }
    }

    private static void LoadLanguage()
    {
        int temp = PlayerPrefs.GetInt(languageKey, 1000);
        
        if (temp == 1000)
        {
            GetLanguage();
            temp = PlayerPrefs.GetInt(languageKey);
        }

        //currentLanguage = languageList[temp];

        //language_Lable.text = MyLocalization.Exchange("current_language");

        //Debug.Log(currentLanguage);
    }

    //public static void ChangeLanguage()
    //{
    //    int temp = PlayerPrefs.GetInt(languageKey);

    //    temp += 1;
    //    temp = temp % languageList.Count;

    //    PlayerPrefs.SetInt(languageKey, temp);
    //    currentLanguage = languageList[temp];

    //    language_Lable.text = MyLocalization.Exchange("current_language");
    //}

    private static void GetLanguage()
    {
        //임시로 1로 세팅한다. 0은 한국어 1은 영어
        PlayerPrefs.SetInt(languageKey, 0);
        Debug.LogError("Language Option Set Needed");
    }
}