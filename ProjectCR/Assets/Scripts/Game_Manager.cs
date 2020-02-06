using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game_Manager : MonoBehaviour
{
    [SerializeField]
    private WealthManager wealthManager = null;
    public Car_Direction car_direction;
    public Track_Manager track_manager;
    public Path_Manager path_manager;
    public Car_Manager car_manager;
    public Canvas canvas;
    public Select_Car select_car;
    public Select_Map select_map;
    public UI_Manager ui_manager;
    public BE.BackendManager backendManager;
    public Record_Manager record_manager;
    public Money_Manager money_manager;
    public IAPManager iapmanager;
    public Store_Manager store_manager;
    public Tip_Manager tip_manager;
    public Sound_Manager sound_manager;
    //private Car_Status current_Selected_Car;
    public Setting_Manager setting_manager;

    [SerializeField]
    private PlayingCarModifier PCModifier = null;

    [SerializeField]
    private Text User_ID_Text;
    [SerializeField]
    private Text Warning_Text;

    // title
    [SerializeField]
    private Image Title_Image;

    [SerializeField]
    private Sprite Eng_Title_Image;

    [SerializeField]
    private Sprite Kor_Title_Image;


    private string Current_Language;
    public string Language
    {
        get
        {
            return Current_Language;
        }
        set
        {
            Current_Language = value;
        }
    }

    public PlayingCarModifier PCM
    {
        get
        {
            return PCModifier;
        }
    }
    public Car_Status Current_Selected_Car
    {
        set
        {
            PCModifier.Initialize(value);
        }
    }
    
    public int ServerTimeChecker = 333;

    private static Game_Manager instance = null;
    public static Game_Manager Instance
    {
        get
        {
            return instance;
        }
    }

    public Dictionary<string, Car_Information> CarStat;

    public Dictionary<string, List<string>> TrackRecord;

    private char[] dumpMemory;

    private void Awake()
    {
        dumpMemory = new char[UnityEngine.Random.Range(0, 500)];

        if( instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        //backendManager.ServerTimer();
        //PlayerPrefs.DeleteAll();
        //Current_Language = Application.systemLanguage.ToString();

        if (PlayerPrefs.GetInt("localized", 0) == 1)
        {
            LanguageOption.Initialize(SystemLanguage.English);
            Title_Image.sprite = Eng_Title_Image;
        }
        else if (PlayerPrefs.GetInt("localized", 0) == 2)
        {
            LanguageOption.Initialize(SystemLanguage.Korean);
            Title_Image.sprite = Kor_Title_Image;
        }
        else
        {
            if(Application.systemLanguage == SystemLanguage.Korean)
            {
                Title_Image.sprite = Kor_Title_Image;
            }
            else
            {
                Title_Image.sprite = Eng_Title_Image;
            }
            LanguageOption.Initialize(Application.systemLanguage);
        }
        User_ID_Text.text = "User ID : " + PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID);
        Warning_Text.text = MyLocalization.Exchange("warning");

    }

    public List<float> GetTrackRecord(string tName)
    {
        List<float> records = new List<float>();

        List<BE.BackendManager.PlayerRankData> rData = new List<BE.BackendManager.PlayerRankData>();

        if (backendManager.TRecord == null)
        {
            return records;
        }
        if (!backendManager.TRecord.ContainsKey(tName))
        {
            return records;
        }

        rData = backendManager.TRecord[tName];

        for (int i = 0; i < rData.Count; i++)
        {
            records.Add(rData[i].score);
        }

        return records;
    }

    public void SetTimeRecord(string tName, float record)
    {
        if (DesignConstStorage.PersonalTrackRecord.ContainsKey(tName))
        {
            DesignConstStorage.PersonalTrackRecord[tName] = record;
            backendManager.SetPersonalRecordToServer(tName);
        }
        else
        {
            Debug.LogError("Wrong Key");
        }
    }
    public void Game_Ready()
    {
        ui_manager.Ingame_camera();
        track_manager.TrackInit();
        PCM.Initialize();
        car_manager.CarInit();
        record_manager.Server_Time();
        record_manager.Reset_Start_Time();
        tip_manager.Random_Tip();
    }

    public void EndOfRace(bool b)
    {
        if (b)
        {
            ui_manager.Finsh_Game();
        }
        else
        {
            ui_manager.Fail_Game();
            //Debug.LogError("레이스 완주 실패");
        }
    }

    public string GetSTime()
    {
        return backendManager.ServerTimer();
    }

    public void Initialize(Text t = null)
    {
        //backendManager.BackeEndTokenLogIn();
        //backendManager.GetPlayerStat();

        //backendManager.WelcomeNewUser();

        //backendManager.GetFreshCarStat();

        const string loadingString = "Loading. . .({0}%)";

        wealthManager.Initialize();

        if(t != null)
        {
            t.text = string.Format(loadingString, 92);
        }
        track_manager.Initialize();

        if (t != null)
        {
            t.text = string.Format(loadingString, 93);
        }
        car_manager.Initialize();

        if (t != null)
        {
            t.text = string.Format(loadingString, 94);
        }
        select_car.Initialize();

        if (t != null)
        {
            t.text = string.Format(loadingString, 96);
        }
        select_map.Initialized();

        if (t != null)
        {
            t.text = string.Format(loadingString, 98);
        }

        sound_manager.Initialized();
        setting_manager.Initialized();

        if (t != null)
        {
            t.text = string.Format(loadingString, 99);
        }

        //storeController가 널인 경우가 있는듯하다
        if (IAPManager.storeController == null)
        {
            StartCoroutine(SCChecker());
        }
        else
        {
            store_manager.Initialized();
        }

        ui_manager.Initialize();
    }

    private IEnumerator SCChecker()
    {
        while(IAPManager.storeController == null)
        {
            iapmanager.GetStoreTableData();

            yield return new WaitForSeconds(1.0f);
        }

        store_manager.Initialized();
    }
}