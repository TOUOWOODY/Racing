using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextLocalization : MonoBehaviour
{
    public Text textAsset = null;

    public string localizationKey = null;
    public string[] localizationValue = null;


    // Use this for initialization
    void Start()
    {
        if (textAsset == null)
        {
            textAsset = GetComponent<Text>();
        }
        
        ExchangeText();
    }
    

    public void ExchangeText()
    {
        if (localizationKey == "" || localizationKey == null)
        {
            Debug.LogError(textAsset.text);
            return;
        }

        if (localizationValue == null)
        {
            textAsset.text = MyLocalization.Exchange(localizationKey);
        }
        else
        {
            switch (localizationValue.Length)
            {
                case 0:
                    textAsset.text = MyLocalization.Exchange(localizationKey);
                    break;
                case 1:
                    textAsset.text = MyLocalization.Exchange(localizationKey, localizationValue[0]);
                    break;
                case 2:
                    textAsset.text = MyLocalization.Exchange(localizationKey, localizationValue[0], localizationValue[1]);
                    break;
                default:
                    Debug.LogError("Too many Value");
                    break;
            }
        }
    }
}