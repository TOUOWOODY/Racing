using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyLocalization
{
    private static TableHandler.Table localizeTable = DesignConstStorage.LocalizeTable;
    //public enum Language {
    //    korean,english
    //}

    //나중에 옵션에서 이거 바꾸는거 만든담에 연동시킨다. 키워드
    //public static Language language = Language.korean;

    public static string Exchange(string origin)
    {
        TableHandler.Row row = localizeTable.FindRow("key", origin);
        string temp;

        //if (LanguageOption.currentLanguage == null)
        //{
        //    LanguageOption.Initialize();
        //}
        if (row != null)
        {
            temp = row.Get<string>(LanguageOption.currentLanguage.ToString());
            //temp = row.Get<string>("English");
        }
        else
        {
            temp = "'" + origin + "' can't find - ";
#if UNITY_EDITOR
            Debug.LogError("Localization Key '" + origin + "' can't find - ");
#endif
        }

#if UNITY_EDITOR
        if (temp.Contains("{"))
        {
            Debug.LogError("String Need More Value. Localize Key is " + origin);
        }
#endif

        return temp;
    }

    public static string Exchange(string origin, string value)
    {

        TableHandler.Row row = localizeTable.FindRow("key", origin);
        string temp;

        //if (LanguageOption.currentLanguage == null)
        //{
        //    LanguageOption.Initialize();
        //}

        if (row != null)
        {
            temp = string.Format(row.Get<string>(LanguageOption.currentLanguage.ToString()), value);
        }
        else
        {
            temp = "'" + origin + "' can't find - ";
            Debug.LogError("Localization Key '" + origin + "' can't find - ");
        }

        if (temp.Contains("{"))
        {
            Debug.LogError("String Need More Value. Localize Key is " + origin);
        }

        return temp;
    }

    public static string Exchange(string origin, string value, string value1)
    {

        TableHandler.Row row = localizeTable.FindRow("key", origin);
        string temp;

        //if (LanguageOption.currentLanguage == null)
        //{
        //    LanguageOption.Initialize();
        //}

        if (row != null)
        {
            temp = string.Format(row.Get<string>(LanguageOption.currentLanguage.ToString()), value, value1);
        }
        else
        {
            temp = "'" + origin + "' can't find - ";
            Debug.LogError("Localization Key '" + origin + "' can't find - ");
        }

        if (temp.Contains("{"))
        {
            Debug.LogError("String Need More Value. Localize Key is " + origin);
        }

        return temp;
    }
}