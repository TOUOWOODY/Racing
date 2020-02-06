using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store_Information
{
    private string price;
    public string Price  // 외부 수정 가능
    {
        get
        {
            return price;
        }
        set
        {
            price = value;
        }
    }

    private WealthManager.WealthType price_type;
    public WealthManager.WealthType Price_Type
    {
        get
        {
            return price_type;
        }
        set
        {
            price_type = value;
        }
    }

    private List<string> value_List = new List<string>();
    public List<string> Value_List
    {
        get
        {
            return value_List;
        }
        set
        {
            value_List = value;
        }
    }

    private List<ItemType> item_List;
    public List<ItemType> Item_List
    {
        get
        {
            return item_List;
        }
        set
        {
            item_List = value;
        }
    }

    public enum ItemType
    {
        cash,
        gamemoney,
        car,
        removead
    }

    private string item_name; // Index
    public string Item_Name
    {
        get
        {
            return item_name;
        }
        set
        {
            item_name = value;
        }
    }

    private string uuid;
    public string UUID
    {
        get
        {
            return uuid;
        }
    }

    private string type;
    public string Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }// Type = 아이탬의 타입 (차 , 스타 , 게임머니 등등)

    private bool isPurchased;
    public bool IsPurchased
    {
        get
        {
            return isPurchased;
        }
        //set
        //{
        //    isPurchased = value;
        //}
    }

    private string category;
    public string Category
    {
        get
        {
            return category;
        }
        set
        {
            category = value;
        }
    }// item
    public bool Initialize(TableHandler.Row data,bool isCar = false)
    {
        bool isFinish = true;

        item_List = new List<ItemType>();

        item_name = data.Get<string>("index");
        category = data.Get<string>("category");

        BE.BackendManager.UIDInfo uInfo = Game_Manager.Instance.backendManager.GetProductUUID(item_name);
        if (uInfo.name == Item_Name)
        {
            //캐시를 써서 구매하는 상품
            uuid = uInfo.uuid;
            price = uInfo.price.ToString();
        }
        else
        {
            //캐시를 사용하지 않는 상품. 현금 or 게임머니 구매상품
        }

        isPurchased = PlayerPrefs.GetString(Item_Name) == Purchased;

        string tcName = "";

        if (isCar)
        {
            item_List.Add(ItemType.car);
            value_List.Add(Item_Name);

            tcName = "wealthtype";
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                value_List.Add(data.Get<string>("value" + i));
                string t = data.Get<string>("item" + i);
                if (t != "")
                {
                    item_List.Add((ItemType)System.Enum.Parse(typeof(ItemType), t));
                }
            }
            
            tcName = "pricetype";
        }

        var p = IAPManager.storeController.products.WithID(item_name);

        if (p != null)
        {
            if (p.metadata.isoCurrencyCode == "KRW")
            {
                price = "￦ " + p.metadata.localizedPrice.ToString();
            }
            else
            {
                price = "$ " + p.metadata.localizedPrice.ToString();
            }
        }

        switch (data.Get<string>(tcName))
        {
            case "realmoney": price_type = WealthManager.WealthType.RealMoney;
                break;
            case "cash": price_type = WealthManager.WealthType.Cash;
                price = uInfo.price.ToString();
                break;
            case "gamemoney": price_type = WealthManager.WealthType.GameMoney;
                price = (data.Get<int>("price") * 2).ToString();
                break;
        }

        //ProvideItems();

        return isFinish;
    }

    private const string Purchased = "Purchased";

    public void ProvideItems()
    {
        for (int i = 0; i < Item_List.Count; i++)
        {
            switch (Item_List[i])
            {
                case ItemType.car:
                    //Debug.LogError(string.Format("sInfo Name : {0}  type : {1}  value : {2}", Item_Name, Item_List[i], value_List[i]));
                    //break;
                    Game_Manager.Instance.backendManager.AddNewCar(Value_List[i]);
                    Game_Manager.Instance.backendManager.GetFreshCarStat();
                    break;
                case ItemType.cash:
                    Game_Manager.Instance.backendManager.GetCash();
                    break;
                case ItemType.gamemoney:
                    //Debug.LogError(string.Format("sInfo Name : {0}  type : {1}  value : {2}", Item_Name, Item_List[i], value_List[i]));
                    //break;
                    int t = System.Convert.ToInt32(Value_List[i]);
                    WealthManager.Instance.IncomeWealth(WealthManager.WealthType.GameMoney, t);
                    break;
                default:
                    Debug.LogError(Item_List[i].ToString());
                    break;
            }
        }

        if (Item_Name.Contains("pack"))
        {
            PlayerPrefs.SetString(Item_Name, Purchased);
        }

        if(Item_List.Contains(ItemType.car))
        {
            for(int i = 0;i< item_List.Count; i++)
            {
                if(item_List[i] == ItemType.car)
                {
                    switch (value_List[i])
                    {
                        case "car_04":
                            PlayerPrefs.SetString("car_04", Purchased);
                            PlayerPrefs.SetString("car_5500", Purchased);
                            PlayerPrefs.SetString("pack_01", Purchased);
                            break;
                        case "car_13":
                            PlayerPrefs.SetString("car_13", Purchased);
                            PlayerPrefs.SetString("car_27500", Purchased);
                            PlayerPrefs.SetString("pack_02", Purchased);
                            break;
                        case "car_14":
                            PlayerPrefs.SetString("car_14", Purchased);
                            PlayerPrefs.SetString("pack_04", Purchased);
                            break;
                        case "car_03":
                            PlayerPrefs.SetString("car_03", Purchased);
                            PlayerPrefs.SetString("pack_00", Purchased);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        isPurchased = PlayerPrefs.GetString(Item_Name) == Purchased;
    }
}