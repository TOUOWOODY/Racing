using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Select_Car : MonoBehaviour
{
    public Slider Topspeed;
    public Slider Acceleration;
    public Slider Cornering;
    public Slider Brake;

    public Text Topspeed_Text;
    public Text Acceleration_Text;
    public Text Cornering_Text;
    public Text Break_Text;
    public Text Carname_Text;
    public Text Car1_T;
    public Text Car2_T;
    public Text Car3_T;
    public Text Car_Page_T;
    public Text Buy_Btn_Text;
    public Text Topspeed_Up_Text;
    public Text Acceleration_Up_Text;
    public Text Cornering_Up_Text;
    public Text Break_Up_Text;
    [SerializeField]
    private Text current_Car_Text;

    [SerializeField]
    private Text current_Car_Text2;

    private int Index;
    private int page;
    private int car_Count;
    private int car, topspeeddone, acceleratingdone, corneringdone, brakedone;

    public string carname;

    public GameObject Car_Btn_1;
    public GameObject Car_Btn_2;
    public GameObject Car_Btn_3;
    public Image Car1_Image;
    public Image Car2_Image;
    public Image Car3_Image;
    public GameObject Select_Btn;
    public GameObject Buy_Btn;
    public GameObject Upgread_TS;
    public GameObject Upgread_AS;
    public GameObject Upgread_CO;
    public GameObject Upgread_BR;
    public GameObject Car;

    [SerializeField]
    private GameObject My_Car_UI;

    [SerializeField]
    private GameObject Menu_UI;


    private TableHandler.Row carData;
    private TableHandler.Row carStat;

    private Select_Car stat_Displayer;

    public Dictionary<string, Car_Status> car_stat;

    private List<string> Key = new List<string>();
    
    public Image Select_Car_Image;
    public Image Menu_Car_Image;

    [SerializeField]
    private Sprite Money_Image;
    [SerializeField]
    private Sprite Cash_Image;
    [SerializeField]
    private Image Money_Icon;

    private const string path = "Image/CarOutFit/{0}";



    void Start()
    {
        
    }
    

    public int TopSpeedDone
    {
        get
        {
            return topspeeddone;
        }
    }

    public int AcceleratingDone
    {
        get
        {
            return acceleratingdone;
        }
    }

    public int CorneringDone
    {
        get
        {
            return corneringdone;
        }
    }

    public int BrakeDone
    {
        get
        {
            return brakedone;
        }
    }

    //public void Initialized() // 게임시작할때 한번 
    //{
    //    car_list = new List<Car_Information>();
    //    for (int i = 0; i < DesignConstStorage.carData.Rows.Count; i++)
    //    {
    //        make_car.Initialize(this, DesignConstStorage.carData.Rows[i]);
    //        car_list.Add(make_car);
    //    }
    //}



       
    private void Car_Image_Text(int i)
    {
        Car1_Image.sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i]].CarName));
        Car2_Image.sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i + 1]].CarName));
        Car3_Image.sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i + 2]].CarName));
        Car1_T.text = MyLocalization.Exchange(car_stat[Key[i]].CarName);
        Car2_T.text = MyLocalization.Exchange(car_stat[Key[i + 1]].CarName);
        Car3_T.text = MyLocalization.Exchange(car_stat[Key[i + 2]].CarName);
        Car_Page_T.text = ((i / 3) + 1) + " / " + page;
    }
    public void Initialize()
    {
        car_Count = DesignConstStorage.CarDataTable.Rows.Count;

        if (car_Count % 3 == 0)
        {
            page = car_Count / 3;
        }
        else
        {
            page = (car_Count / 3) + 1;
        }

        car_stat = new Dictionary<string, Car_Status>();

        for (int i = 0; i < DesignConstStorage.CarDataTable.Rows.Count; i++)
        {
            Car_Status car_Status = new Car_Status();
            car_Status.Initialize(DesignConstStorage.CarDataTable.Rows[i]);
            car_stat.Add(car_Status.CarName , car_Status);
            Key.Add(car_Status.CarName);
        }

        for (int i = 0; i < car_stat.Count; i++)
        {
            if (car_stat[Key[i]].CarName == PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName))
            {
                Game_Manager.Instance.Current_Selected_Car = car_stat[Key[i]];

                StatSliderAndText(i);
                GetCarStatus(i);
                Index = i;
            }
        }
        car = PlayerPrefs.GetInt("car", 0);
        Car_Image_Text(car);
        Debug.LogWarning(PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName));
        Select_Car_Image.sprite = Resources.Load<Sprite>(string.Format(path, PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName)));
        Menu_Car_Image.sprite = Resources.Load<Sprite>(string.Format(path, PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName)));
        Car.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Format(path, PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName)));
    }
    private void GetCarStatus(int index)
    {
        topspeeddone = car_stat[Key[index]].Information.TSD;
        acceleratingdone = car_stat[Key[index]].Information.ACD;
        corneringdone = car_stat[Key[index]].Information.COD;
        brakedone = car_stat[Key[index]].Information.BRD;

        Topspeed_Up_Text.text = CalcUpgradePrice(index, topspeeddone).ToString();
        Acceleration_Up_Text.text = CalcUpgradePrice(index, acceleratingdone).ToString();
        Cornering_Up_Text.text = CalcUpgradePrice(index, corneringdone).ToString();
        Break_Up_Text.text = CalcUpgradePrice(index, brakedone).ToString();

        current_Car_Text.text = MyLocalization.Exchange("currentcartext") + " : " + MyLocalization.Exchange(car_stat[Key[index]].CarName);
        current_Car_Text2.text = current_Car_Text.text;
    }
    
    public void Click_TopSpeed_Upgread()
    {
        Upgread(DesignConstStorage.StatList.topspeed);
    }
    public void Click_Accelerating_Upgread()
    {
        Upgread(DesignConstStorage.StatList.accelerating);
    }
    public void Click_Cornering_Upgread()
    {
        Upgread(DesignConstStorage.StatList.cornering);
    }
    public void Click_Break_Upgread()
    {
        Upgread(DesignConstStorage.StatList.brake);
    }

    
    private void Upgread(DesignConstStorage.StatList stat)
    {
        int stat_upgread;

        switch (stat)
        {
            case DesignConstStorage.StatList.topspeed:
                stat_upgread = car_stat[Key[Index]].Information.TSD;
                break;
            case DesignConstStorage.StatList.accelerating:
                stat_upgread = car_stat[Key[Index]].Information.ACD;
                break;
            case DesignConstStorage.StatList.cornering:
                stat_upgread = car_stat[Key[Index]].Information.COD;
                break;
            case DesignConstStorage.StatList.brake:
                stat_upgread = car_stat[Key[Index]].Information.BRD;
                break;
            default:
                stat_upgread = 0;
                break;
        }

        int Upgread_Price = CalcUpgradePrice(stat_upgread);
        if (stat_upgread < DesignConstStorage.MaxUpgrade && WealthManager.Instance.SpendWealth(WealthManager.WealthType.GameMoney, Upgread_Price))
        {
            Game_Manager.Instance.backendManager.IncreaseStat(car_stat[Key[Index]].Information, stat);
            GetCarStatus(Index);
            StatSliderAndText(Index);
            Game_Manager.Instance.ui_manager.Menu_Car();
        }
    }

    private int CalcUpgradePrice(int u)
    {
        TableHandler.Row row = TableHandler.Get(DesignConstStorage.tNameCarPrice, TableHandler.SteamMode.Resource).Rows[Index];

        int carPrice = row.Get<int>("price") * 2;

        int price = (int)(carPrice * 0.1f) * (u + 1);

        return price;
    }

    private int CalcUpgradePrice(int i, int u)
    {
        int price = (int)(car_stat[Key[i]].Price * 0.1f) * (u + 1);

        return price;
    }

    public void Click_Car()
    {
        for (int i = 0; i < car_stat.Count; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text == MyLocalization.Exchange(car_stat[Key[i]].CarName))
            {
                Index = i;
                Current_Active_Btn(i);
                StatSliderAndText(i);
                GetCarStatus(i);
                Select_Car_Image.sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i]].CarName));
            }
        }
    }
    private void Current_Active_Btn(int i)
    {
        //Debug.LogWarning("123" + car_stat[Key[i]].Information.IsPurchased);

        if (car_stat[Key[i]].Information.IsPurchased)
        {
            //Debug.LogWarning(car_stat[Key[i]].Information.IsPurchased);
            Index = i;
            Upgread_TS.gameObject.SetActive(true);
            Upgread_AS.gameObject.SetActive(true);
            Upgread_CO.gameObject.SetActive(true);
            Upgread_BR.gameObject.SetActive(true);
            //PlayerPrefs.SetString("select_car", car_stat[Key[i]].CarName);

            Select_Btn.gameObject.SetActive(true);
            Buy_Btn.gameObject.SetActive(false);
        }
        else
        {
            Upgread_TS.gameObject.SetActive(false);
            Upgread_AS.gameObject.SetActive(false);
            Upgread_CO.gameObject.SetActive(false);
            Upgread_BR.gameObject.SetActive(false);

            Buy_Btn_Text.text = Game_Manager.Instance.store_manager.Store_Dic[car_stat[Key[i]].CarName].Price;

            switch(Game_Manager.Instance.store_manager.Store_Dic[car_stat[Key[i]].CarName].Price_Type)
            {
                case WealthManager.WealthType.Cash:
                    Money_Icon.sprite = Cash_Image;
                    break;
                case WealthManager.WealthType.GameMoney:
                    Money_Icon.sprite = Money_Image;
                    break;
                case WealthManager.WealthType.RealMoney:
                    break;
            }

            if(Game_Manager.Instance.store_manager.Store_Dic[car_stat[Key[i]].CarName].Price_Type == WealthManager.WealthType.RealMoney)
            {
                Money_Icon.gameObject.SetActive(false);
                Buy_Btn_Text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                Money_Icon.gameObject.SetActive(true);
                Buy_Btn_Text.GetComponent<RectTransform>().localPosition = new Vector3(64.5f, 0, 0);
            }

            //Debug.LogError(Buy_Btn_Text.text);
            Select_Btn.gameObject.SetActive(false);
            Buy_Btn.gameObject.SetActive(true);
        }
            //Buy_Btn_Text.text = MyLocalization.Exchange(DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype")) + DesignConstStorage.CarDataTable.Rows[i].Get<int>("price");

            //Buy_Btn_Text.text = DesignConstStorage.LocalizeTable.FindRow("key", DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype")).ToString() + DesignConstStorage.CarDataTable.Rows[i].Get<int>("price");
            //switch (DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype"))
            //{
            //    case "gamemoney":
            //        Buy_Btn_Text.text = MyLocalization.Exchange(DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype")) + Game_Manager.Instance.store_manager.Store_Dic[car_stat[Key[i]].CarName].Price;
            //        break;
            //    case "cash":
            //        Buy_Btn_Text.text = MyLocalization.Exchange(DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype")) + DesignConstStorage.CarDataTable.Rows[i].Get<int>("price");
            //        break;
            //    case "realmoney":
            //        Buy_Btn_Text.text = MyLocalization.Exchange(DesignConstStorage.CarDataTable.Rows[i].Get<string>("wealthtype")) + ""; // 구글에서 진짜 돈 가져와야됨. 
            //        break;
            //}
    }

    //public void Click_Buy_Car()
    //{
    //    switch (DesignConstStorage.CarDataTable.Rows[Index].Get<string>("wealthtype"))
    //    {
    //        case "gamemoney":
    //            Buy_Car(Index, WealthManager.WealthType.GameMoney);
    //            break;

    //        case "cash":
    //            Buy_Car(Index, WealthManager.WealthType.Cash);
    //            break;

    //        case "realmoney":
    //            Buy_Car(Index, WealthManager.WealthType.RealMoney);
    //            break;
    //    }
    //}

    public void Buy_Car() // int i ,WealthManager.WealthType type
    {
        Game_Manager.Instance.iapmanager.PurchaseProduct(Game_Manager.Instance.store_manager.Store_Dic[car_stat[Key[Index]].CarName]);
        Current_Active_Btn(Index);
        //if (WealthManager.Instance.SpendWealth(type, 0))//DesignConstStorage.CarDataTable.Rows[i].Get<int>("price")
        //{
        //    Debug.LogError(DesignConstStorage.CarDataTable.Rows[i].Get<int>("price"));
        //    Game_Manager.Instance.backendManager.AddNewCar(car_stat[Key[i]].CarName);
        //    Game_Manager.Instance.backendManager.GetFreshCarStat();
        //}
    }
    public void Click_N_Btn()
    {
        if (car_stat.Count > car + 3)
        {
            car += 3;
            Car1_T.text = MyLocalization.Exchange(car_stat[Key[car]].CarName);
            car_active(car_stat.Count % 3);
            //switch (car_stat.Count % 3)
            //{
            //    case 0:
            //        Car2_T.text = MyLocalization.Exchange(car_stat[Key[car + 1]].CarName);
            //        Car3_T.text = MyLocalization.Exchange(car_stat[Key[car + 2]].CarName);
            //        break;
            //    case 1:
            //        break;
            //    case 2:
            //        Car2_T.text = MyLocalization.Exchange(car_stat[Key[car + 1]].CarName);
            //        break;
            //}

        }
        Car_Image_Text(car);
    }

   

    public void Click_P_Btn()
    {
        if (car != 0)
        {
            car -= 3;
            car_active(0);
            //Car1_T.text = MyLocalization.Exchange(car_stat[Key[car]].CarName);
            //Car2_T.text = MyLocalization.Exchange(car_stat[Key[car + 1]].CarName);
            //Car3_T.text = MyLocalization.Exchange(car_stat[Key[car + 2]].CarName);

        }
        Car_Image_Text(car);
    }

    void Update()
    {
     
    }

    public void click_select()
    {
        for(int i = 0; i < car_stat.Count; i++)
        {
            if (car_stat[Key[i]].CarName == car_stat[Key[Index]].CarName)
            {
                Game_Manager.Instance.Current_Selected_Car = car_stat[Key[i]];
                Car.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i]].CarName));
                Menu_Car_Image.sprite = Resources.Load<Sprite>(string.Format(path, car_stat[Key[i]].CarName));
                PlayerPrefs.SetString("select_car", car_stat[Key[i]].CarName);
            }
        }
        GetCarStatus(Index);
        Game_Manager.Instance.ui_manager.Menu_Car();
        PlayerPrefs.SetInt("car", car);
        PlayerPrefs.SetInt("index", Index);
    }

    private const string notPurchased = "{0} : {1}";
    private const string Purchased = "{0} : {1} (+{2})";

    public void Click_Cancle()
    {
        StatSliderAndText(PlayerPrefs.GetInt("index", 0));
        GetCarStatus(PlayerPrefs.GetInt("index", 0));
    }
    private void StatSliderAndText(int i)
    {
        Topspeed.maxValue = car_stat[Key[i]].TopSpeed + (car_stat[Key[i]].Tsc * 10);
        Acceleration.maxValue = car_stat[Key[i]].Acceleration + (car_stat[Key[i]].Acc * 10);
        Cornering.maxValue = car_stat[Key[i]].Cornering + (car_stat[Key[i]].Coc * 10);
        Brake.maxValue = car_stat[Key[i]].Brake + (car_stat[Key[i]].Brc * 10);

        Topspeed.value = car_stat[Key[i]].TotalTS;
        Acceleration.value = car_stat[Key[i]].TotalAC;
        Cornering.value = car_stat[Key[i]].TotalCO;
        Brake.value = car_stat[Key[i]].TotalBR;
        Carname_Text.text = MyLocalization.Exchange(car_stat[Key[i]].CarName);

        if (car_stat[Key[i]].Information.IsPurchased)
        {
            Topspeed_Text.text = string.Format(Purchased, MyLocalization.Exchange("topspeed"), Topspeed.value, car_stat[Key[i]].Tsc);
            Acceleration_Text.text = string.Format(Purchased, MyLocalization.Exchange("acceleration"), Acceleration.value, car_stat[Key[i]].Acc);
            Cornering_Text.text = string.Format(Purchased, MyLocalization.Exchange("cornering"), Cornering.value, car_stat[Key[i]].Coc);
            Break_Text.text = string.Format(Purchased, MyLocalization.Exchange("braking"), Brake.value, car_stat[Key[i]].Brc);
        }
        else
        {
            Topspeed_Text.text = string.Format(notPurchased, MyLocalization.Exchange("topspeed"), Topspeed.value);
            Acceleration_Text.text = string.Format(notPurchased, MyLocalization.Exchange("acceleration"), Acceleration.value);
            Cornering_Text.text = string.Format(notPurchased, MyLocalization.Exchange("cornering"), Cornering.value);
            Break_Text.text = string.Format(notPurchased, MyLocalization.Exchange("braking"), Brake.value);
        }
    }

    private void car_active(int i) // i 는 현재 페이지 차 갯수
    {
        switch(i)
        {
            case 0:
                Car_Btn_1.gameObject.SetActive(true);
                Car_Btn_2.gameObject.SetActive(true);
                Car_Btn_3.gameObject.SetActive(true);
                break;
            case 1:
                Car_Btn_1.gameObject.SetActive(true);
                Car_Btn_2.gameObject.SetActive(false);
                Car_Btn_3.gameObject.SetActive(false);
                break;
            case 2:
                Car_Btn_1.gameObject.SetActive(true);
                Car_Btn_2.gameObject.SetActive(true);
                Car_Btn_3.gameObject.SetActive(false);
                break;
        }
    }

    public void Click_My_Car()
    {
        car = PlayerPrefs.GetInt("car", 0);
        Car_Image_Text(car);
        My_Car_UI.gameObject.SetActive(true);
        Menu_UI.gameObject.SetActive(false);
        Select_Car_Image.sprite = Resources.Load<Sprite>(string.Format(path, PlayerPrefs.GetString("select_car", car_stat[Key[0]].CarName)));
        Current_Active_Btn(PlayerPrefs.GetInt("index", 0));
        GetCarStatus(PlayerPrefs.GetInt("index", 0));
        StatSliderAndText(PlayerPrefs.GetInt("index", 0));
    }

}
