using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Record_Manager : MonoBehaviour
{
    public bool Record;
    public bool Start_Record_Bool;

    public float Record_Time;
    private string start_time, finish_time;
    private int detection_start, detection_finish, server_sec;
    public Text Total_Record_Text, Record_Text, Penalty_Text, Current_Speed_Text;

    [SerializeField]
    private Text Start_Interval_Text;

    [SerializeField]
    private GameObject booster_Image;

    private float start_Interval = 0;

    private float BoosterTime = 0;

    [SerializeField]
    private Text Penalty_Bonus_Text;
    void Start()
    {
        
    }


    void Update()
    {
        Game_Manager.Instance.ui_manager.Record_Text.text = "TIME : " + Record_Time.ToString("N3") + " sec";
        if (Record)
        {
            Record_Time += Time.deltaTime;
        }
        if(Record_Time >= 59.999)
        {
            Record_Time = 59.999f;
        }
        Current_Speed_Text.text = (int)Game_Manager.Instance.car_direction.CurrentSpeed + " km/h";
    }

    public void Check_Start_Interval()
    {
        if(Start_Record_Bool)
        {
            start_Interval = Record_Time;
            Start_Interval_Text.text = "START Interval : " + start_Interval.ToString("N3") + " sec";
        }

        Start_Record_Bool = false;

        if(start_Interval < 0.25f && !Game_Manager.Instance.car_manager.IsOnCountDown)
        {
            Game_Manager.Instance.car_direction.IsBooster = true;
            booster_Image.SetActive(true);

            BoosterTime = 0.5f + (0.5f - ((start_Interval / 0.0025f) * 0.005f));
            Debug.LogWarning("booster time : " + BoosterTime);
            StartCoroutine(booster());
        }
    }

    IEnumerator booster()
    {
        while(Record_Time < BoosterTime)
        {
            yield return new WaitForSeconds(0.01f);
        }
        Game_Manager.Instance.car_direction.IsBooster = false;
        booster_Image.SetActive(false);
    }

    public void Reset_Start_Time()
    {
        Start_Interval_Text.text = "START Interval : 0.000 sec";
        Start_Record_Bool = true;
        booster_Image.SetActive(false);
    }

    public void Racing_Record_Start()
    {
        Record = true;
        start_time = System.DateTime.Now.ToString("HHmmssfff");
        detection_start = int.Parse(start_time) % Game_Manager.Instance.ServerTimeChecker;
    }
    
    public void Server_Time()
    {
        char[] aaa = Game_Manager.Instance.GetSTime().ToCharArray();
        string server = aaa[32] + aaa[33] + aaa[34] + "";
        server_sec = int.Parse(server);
    }
    public void Racing_Record_Finish()
    {
        finish_time = System.DateTime.Now.ToString("HHmmssfff");
        int result = (int.Parse(finish_time) - (int)(Record_Time * 1000f));
        if(Record_Time >= 59.999f)
        {
            Record_Time = 59.999f;
        }
        if(int.Parse(start_time) - 100 <= result || result <= int.Parse(start_time) + 100)
        {
            if(detection_start == int.Parse(start_time) % Game_Manager.Instance.ServerTimeChecker)
            {
                Total_Record_Text.text = "00 : " + Record_Time.ToString("N3");
                if (Game_Manager.Instance.car_direction.Bonus)
                {
                    Penalty_Text.text = "    -  " + Game_Manager.Instance.car_direction.Penalty.ToString("N3");
                    Penalty_Text.GetComponent<Text>().color = new Color(100 / 255f, 220 / 255f, 0 / 255f);
                    Record_Text.text = "00 : " + (Record_Time + Game_Manager.Instance.car_direction.Penalty).ToString("N3");
                    Penalty_Bonus_Text.text = MyLocalization.Exchange("bonus");
                }
                else if (Game_Manager.Instance.car_direction.Bonus == false)
                {
                    Penalty_Text.text = "    +  " + Game_Manager.Instance.car_direction.Penalty.ToString("N3");
                    Penalty_Text.GetComponent<Text>().color = new Color(255 / 255f, 0 / 255f, 0 / 255f);
                    Record_Text.text = "00 : " + (Record_Time - Game_Manager.Instance.car_direction.Penalty).ToString("N3");
                    Penalty_Bonus_Text.text = MyLocalization.Exchange("penalty");
                }
                else if (Game_Manager.Instance.car_direction.Bonus == false && Game_Manager.Instance.car_direction.Penalty == 0)
                {
                    Penalty_Text.gameObject.SetActive(false);
                }

                //DesignConstStorage.PersonalTrackRecord.Add(DesignConstStorage.TrackDataTable.Rows[PlayerPrefs.GetInt("select", 0)].Get<string>("index"), Record_Time);
            }
        }
        else
        {
            Debug.Log("기록 조작 의심.");
        }
    }
}
