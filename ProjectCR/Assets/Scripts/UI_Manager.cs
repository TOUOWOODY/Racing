using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
    public GameObject Menu_UI;
    public GameObject My_Car_UI;
    public GameObject Track_UI;
    public GameObject Ingame_UI;
    public GameObject Finsh_UI;
    public GameObject Fail_Game_UI;
    public GameObject Track_Postion;
    public GameObject Roading_UI;
    public GameObject Stroe_UI;
    public GameObject Select_Map_Record_Parents;
    public GameObject Menu_Record_Parents;


    public GameObject AD_Btn; // 광고 버튼

    public GameObject x2_Btn; // 광고 후 보상 버튼

    public GameObject Reward_Btn;

    public Camera UI_Camera, Main_Camera;
    public Transform mainmap;
    public Transform Select_map;
    public Transform[] track = new Transform[3];

    [SerializeField]
    private Text Topspeed_Text;
    [SerializeField]
    private Text Acceleration_Text;
    [SerializeField]
    private Text Cornering_Text;
    [SerializeField]
    private Text Break_Text;
    [SerializeField]
    private Text Carname_Text;
    [SerializeField]
    private Text Go_Menu_Text;

    public Text Record_Text;
    public Text Fail_Racing_Reason_Text;

    public Text x2_Btn_Text;
    public Slider Topspeed, Acceleration, Cornering, Brake;
    public int money;

    public bool ads;

    private bool removeAD;


    [SerializeField]
    private GameObject car;
    [SerializeField]
    private GameObject ingame_BackGround;

    public void Initialize()
    {
        ads = false;
        Menu_UI.transform.Reset();
        My_Car_UI.transform.Reset();
        Track_UI.transform.Reset();
        Ingame_UI.transform.Reset();
        Stroe_UI.transform.Reset();

        Menu_UI.gameObject.SetActive(true);
        My_Car_UI.gameObject.SetActive(false);
        Track_UI.gameObject.SetActive(false);
        Ingame_UI.gameObject.SetActive(false);
        Roading_UI.gameObject.SetActive(false);
        Stroe_UI.gameObject.SetActive(false);

        Outgame_camera();
        Main_Camera.transform.Reset();
        Main_Camera.orthographicSize = 57.74f;

        Menu_Car();
        Select_map.GetComponent<MeshFilter>().mesh = Game_Manager.Instance.track_manager.GetMapMesh(map_name(PlayerPrefs.GetInt("select", 0)));
        mainmap.GetComponent<MeshFilter>().mesh = Select_map.GetComponent<MeshFilter>().mesh;
        Select_map.GetComponent<MeshRenderer>().material = Material(map_name(PlayerPrefs.GetInt("select", 0))).sharedMaterial;
        Game_Manager.Instance.track_manager.material.material = Select_map.GetComponent<MeshRenderer>().material;
        for (int i = 0; i <  3; i++)
        {
            track[i].transform.localScale = track_scale(map_name(i));
        }

        mainmap.localScale = track_scale(map_name(PlayerPrefs.GetInt("select", 0))) * new Vector2(1.8f, 1.8f);
        Select_map.localScale = mainmap.localScale;
    }

    public Vector2 track_scale(string map)
    {
        float size;
        if (Game_Manager.Instance.track_manager.GetMapRect(map).width >= Game_Manager.Instance.track_manager.GetMapRect(map).height)
        {
            size = Game_Manager.Instance.track_manager.GetMapRect(map).width;
        }
        else
        {
            size = Game_Manager.Instance.track_manager.GetMapRect(map).height;
        }

        return new Vector2(115 / size, 115 / size);
    }

    public MeshRenderer Material(string map)
    {
        //.transform.GetChild(0).GetComponent<MeshRenderer>()
        return Resources.Load<GameObject>(string.Format("Prefab/Maps/{0}", map)).transform.GetChild(0).GetComponent<MeshRenderer>();
    }



    public void Ingame_camera()
    {
        UI_Camera.depth = 2;
        Game_Manager.Instance.money_manager.Money_Active();
        ads = false;

        ingame_BackGround.SetActive(true);
        car.SetActive(true);
    }
    public void Outgame_camera()
    {
        UI_Camera.depth = 0;
        Game_Manager.Instance.money_manager.Money_Active();

        ingame_BackGround.SetActive(false);
        car.SetActive(false);
    }
    public void Menu_Car()
    {
        Topspeed.maxValue = Game_Manager.Instance.select_car.Topspeed.maxValue;
        Acceleration.maxValue = Game_Manager.Instance.select_car.Acceleration.maxValue;
        Cornering.maxValue = Game_Manager.Instance.select_car.Cornering.maxValue;
        Brake.maxValue = Game_Manager.Instance.select_car.Brake.maxValue;


        Topspeed.value = Game_Manager.Instance.select_car.Topspeed.value;
        Acceleration.value = Game_Manager.Instance.select_car.Acceleration.value;
        Cornering.value = Game_Manager.Instance.select_car.Cornering.value;
        Brake.value = Game_Manager.Instance.select_car.Brake.value;
        Carname_Text.text = Game_Manager.Instance.select_car.Carname_Text.text;

        Topspeed_Text.text = MyLocalization.Exchange("topspeed") + " : " + Topspeed.value;
        Acceleration_Text.text = MyLocalization.Exchange("acceleration") + " : " + Acceleration.value;
        Cornering_Text.text = MyLocalization.Exchange("cornering") + " : " + Cornering.value;
        Break_Text.text = MyLocalization.Exchange("braking") + " : " + Brake.value;
    }

    public void Finsh_Game()
    {
        Game_Manager.Instance.record_manager.Record = false;
        Finsh_UI.gameObject.SetActive(true);
        Track_Postion = null;
        Game_Manager.Instance.record_manager.Racing_Record_Finish();
        Game_Manager.Instance.select_map.PersonalRecord(PlayerPrefs.GetInt("select", 0));
        Game_Manager.Instance.record_manager.Penalty_Text.gameObject.SetActive(true);

        Game_Manager.Instance.sound_manager.audioSource.Stop();


        if(Game_Manager.Instance.store_manager.IsAdRemove)
        {
            AD_Btn.gameObject.SetActive(false);
            Reward_Btn.gameObject.SetActive(false);
            x2_Btn.gameObject.SetActive(true);

            x2_Btn_Text.text = (money * 2) + "";
        }
        else
        {
            AD_Btn.gameObject.SetActive(true);
            Reward_Btn.gameObject.SetActive(true);
            x2_Btn.gameObject.SetActive(false);

            Go_Menu_Text.text = money + "";
        }
    }
    public void Fail_Game()
    {
        Game_Manager.Instance.record_manager.Record = false;
        Fail_Game_UI.gameObject.SetActive(true);
        Track_Postion = null;
        Game_Manager.Instance.car_manager.IsOnCountDown = false;

        Game_Manager.Instance.sound_manager.audioSource.Stop();
    }

    public void Click_Fail_Racing()
    {
        Reset_Racing();
    }
    public void Click_Success_Racing()
    {
        if (ads)
        {
            WealthManager.Instance.IncomeWealth(WealthManager.WealthType.GameMoney,  money * 2);
        }
        else
        {
            WealthManager.Instance.IncomeWealth(WealthManager.WealthType.GameMoney, money);
        }

        Reset_Racing();
        ads = false;
        AD_Btn.gameObject.SetActive(true);
        Reward_Btn.gameObject.SetActive(true);
        x2_Btn.gameObject.SetActive(false);
    }

    private void Reset_Racing()
    {
        Menu_UI.gameObject.SetActive(true);
        Ingame_UI.gameObject.SetActive(false);
        Finsh_UI.gameObject.SetActive(false);
        Fail_Game_UI.gameObject.SetActive(false);
        Main_Camera.transform.Reset();
        Main_Camera.orthographicSize = 57.74f;
        Game_Manager.Instance.track_manager.Track_Reset();
        Game_Manager.Instance.car_manager.car_direstion.SetState();
        Initialize();
        Outgame_camera();
    }


    private string map_name(int i)
    {
        return DesignConstStorage.TrackDataTable.Rows[i].Get<string>("mapname");
    }
}
