using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Setting_Manager : MonoBehaviour
{

    [SerializeField]
    private Text user_ui;
    [SerializeField]
    private Text Lan_Text;

    [SerializeField]
    private GameObject track0;
    [SerializeField]
    private GameObject track1;
    [SerializeField]
    private GameObject track2;
    [SerializeField]
    private GameObject track3;
    [SerializeField]
    private GameObject track4;
    [SerializeField]
    private GameObject Setting_UI;

    [SerializeField]
    private Toggle easy_Mode_Btn;
    [SerializeField]
    private Toggle sound_Btn;

    private SystemLanguage Language = LanguageOption.currentLanguage;

    public GameObject[] Text_arr;

    private bool auto_Start = true;

    [SerializeField]
    private Text version_Text;
    public bool Auto_Start
    {
        get
        {
            return auto_Start;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void Start()
    {
    }

    public void Initialized()
    {
        version_Text.text = "v " + Application.version;
        if (PlayerPrefs.GetInt("easy_mode", 1) == 0)
        {
            auto_Start = false;
            easy_Mode_Btn.isOn = false;
        }
        else
        {
            auto_Start = true;
            easy_Mode_Btn.isOn = true;
        }


        if (PlayerPrefs.GetInt("sound", 1) == 0)
        {
            Game_Manager.Instance.sound_manager.audioSource.mute = true;
            sound_Btn.isOn = false;
        }
        else
        {
            Game_Manager.Instance.sound_manager.audioSource.mute = false;
            sound_Btn.isOn = true;
        }


        user_ui.text = "Player ID"+ "\n" + PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID);

        if (PlayerPrefs.GetInt("localized", 0) == 1)
        {
            Lan_Text.text = "ENG";
        }
        else if (PlayerPrefs.GetInt("localized", 0) == 2)
        {
            Lan_Text.text = "KOR";
        }
        else
        {
            if(Language == SystemLanguage.Korean)
            {
                Lan_Text.text = "KOR";
            }
            else
            {
                Lan_Text.text = "ENG";
            }
        }

    }
    public void Sound_Btn()
    {
        if(sound_Btn.isOn)
        {
            PlayerPrefs.SetInt("sound", 1);
            Game_Manager.Instance.sound_manager.audioSource.mute = false;// 켜짐
        }
        else
        {
            PlayerPrefs.SetInt("sound", 0);
            Game_Manager.Instance.sound_manager.audioSource.mute = true;
        }
    }

    public void Easy_Mode_Btn()
    {
        if (easy_Mode_Btn.isOn)
        {
            PlayerPrefs.SetInt("easy_mode", 1);
            auto_Start = true;
        }
        else
        {
            PlayerPrefs.SetInt("easy_mode", 0);
            auto_Start = false;
        }
    }

    public void Localize()
    {
        switch (Language)
        {
            case SystemLanguage.Korean:
                Language = SystemLanguage.English;
                Lan_Text.text = "ENG";
                PlayerPrefs.SetInt("localized", 1);
                break;
            case SystemLanguage.English:
                Language = SystemLanguage.Korean;
                Lan_Text.text = "KOR";
                PlayerPrefs.SetInt("localized", 2);
                break;
            default:
                Language = SystemLanguage.English;
                Lan_Text.text = "ENG";
                PlayerPrefs.SetInt("localized", 1);
                break;
        }

        LanguageOption.Initialize(Language);
        for (int i = 0; i < Text_arr.Length - 1; i++)
        {
            Text_arr[i].GetComponent<TextLocalization>().textAsset.GetComponent<Text>().text = MyLocalization.Exchange(Text_arr[i].GetComponent<TextLocalization>().localizationKey);
        }
        Game_Manager.Instance.Initialize(null);
    }

    // activeSelf로 설정하면 반응이 안올때가 있음..
    public void Click_Setting()
    {
        Setting_UI.SetActive(true);
        if(Setting_UI.activeSelf)
        {
            track0.SetActive(false);
            track1.SetActive(false);
            track2.SetActive(false);
            track3.SetActive(false);
            track4.SetActive(false);
        }
    }

    public void Exit_Setting()
    {
        Setting_UI.SetActive(false);
        if (!Setting_UI.activeSelf)
        {
            track0.SetActive(true);
            track1.SetActive(true);
            track2.SetActive(true);
            track3.SetActive(true);
            track4.SetActive(true);
        }
    }
}
