using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Store_Manager : MonoBehaviour
{
    [SerializeField]
    private Image Select_Item_Image;

    [SerializeField]
    private GameObject Category;

    [SerializeField]
    private GameObject Item;

    [SerializeField]
    private GameObject Category_Parents;

    [SerializeField]
    private GameObject Item_Parents;

    [SerializeField]
    private GameObject Inforamtion_Parents;

    [SerializeField]
    private GameObject Menu_UI;

    [SerializeField]
    private GameObject Store_UI;

    [SerializeField]
    private GameObject Buy_Image;

    [SerializeField]
    private Text Buy_Btn_Text;

    private List<string> category_List = new List<string>();
    public Dictionary<string, Store_Information> Store_Dic = null;
    private List<string> Store_Key = null;

    private GameObject item;

    private List<GameObject> Select_Item_List = new List<GameObject>();

    private List<GameObject> information_List = new List<GameObject>();

    private GameObject select_Item;

    private bool isAdRemoved
    {
        get
        {
            return Store_Dic["pack_03"].IsPurchased;
        }
    }
    public bool IsAdRemove
    {
        get
        {
            return isAdRemoved;
        }
    }

    private int current_num;

    private int first_item;
    public void Initialized()
    {
        Store_Dic = new Dictionary<string, Store_Information>();
        
        for(int i = 0; i < 4; i++)
        {
            information_List.Add(Inforamtion_Parents.transform.GetChild(i).gameObject);
        }
        //게임 시작시 처음 상점 세팅

        for (int i = 0; i < DesignConstStorage.StoreTable.Rows.Count; i++)
        {
            Store_Information store_information = new Store_Information();

            bool isInitialized = store_information.Initialize(DesignConstStorage.StoreTable.Rows[i]);

            if (isInitialized)
            {
                Store_Dic.Add(store_information.Item_Name, store_information);
            }
        }
        
        for(int i = 0; i < DesignConstStorage.CarPriceTable.Rows.Count; i++)
        {
            Store_Information store_information = new Store_Information();
            switch (DesignConstStorage.CarPriceTable.Rows[i].Get<string>("wealthtype"))
            {
                case "realmoney":
                    string index = DesignConstStorage.CarPriceTable.Rows[i].Get<string>("index");
                    switch (index)
                    {
                        case DesignConstStorage.RealMoneyCarName5500:
                            if (Store_Dic.ContainsKey("car_5500"))
                            {
                                store_information = Store_Dic["car_5500"];
                            }
                            break;
                        case DesignConstStorage.RealMoneyCarName27500:
                            if (Store_Dic.ContainsKey("car_27500"))
                            {
                                store_information = Store_Dic["car_27500"];
                            }
                            break;
                        default:
                            Debug.LogError("Not Defined Item");
                            break;
                    }
                    if(store_information != null)
                    {
                        Store_Dic.Add(index, store_information);
                    }
                    break;
                default:
                    bool isInitialized = store_information.Initialize(DesignConstStorage.CarPriceTable.Rows[i], true);
                    if (isInitialized)
                    {
                        Store_Dic.Add(store_information.Item_Name, store_information);
                    }
                    break;
            }
        }

        while (Category_Parents.transform.childCount != 0)
        {
            DestroyImmediate(Category_Parents.transform.GetChild(0).gameObject);
        }

        category_List.Clear();

        Store_Key = new List<string>(Store_Dic.Keys);
        for (int i = 0; i < DesignConstStorage.StoreTable.Rows.Count; i++)
        {
            if (category_List.Contains(Store_Dic[Store_Key[i]].Category) == false && !Store_Dic[Store_Key[i]].IsPurchased) // 처음 카테고리 버튼 생성 (중복 x)
            {
                GameObject category = Instantiate(Category, new Vector2(0, 0), Quaternion.identity);
                category.transform.SetParent(Category_Parents.transform, false);
                category.name = Store_Dic[Store_Key[i]].Category;
                category.transform.GetChild(0).GetComponent<Text>().text = MyLocalization.Exchange(DesignConstStorage.StoreTable.Rows[i].Get<string>("category"));
                if(category.transform.GetChild(0).GetComponent<Text>().preferredWidth > 250)
                {
                    category.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(category.transform.GetChild(0).GetComponent<Text>().preferredWidth + 50, 120, 0);
                }
                else
                {
                    category.transform.GetComponent<RectTransform>().sizeDelta = new Vector3(250, 120, 0);
                }
                category_List.Add(category.name);
            }
        }
    }
    
    private void Reset_Item(int i, string category)
    {
        if (Store_Dic[Store_Key[i]].Category == category)
        {

            select_Item = Select_Item_List[0];
            if (first_item >= i)
            {
                first_item = i;
                current_num = i;
                item_Text(i);
                Select_Item_Image.sprite = Select_Item_List[0].transform.GetChild(1).GetComponent<Image>().sprite;
                Buy_Btn_Text.text = Store_Dic[Store_Key[first_item]].Price + "";
                Buy_Image_Active(i);
            }
        }
    }


    private void item_Text(int i)
    {
        for(int j = 0; j < 4; j++)
        {
            if(Store_Dic[Store_Key[i]].Value_List[j] != "")
            {
                information_List[j].gameObject.SetActive(true);
                //information_List[j].transform.GetComponent<Text>().text = Store_Dic[Store_Key[i]].Value_List[j];
                if(Store_Dic[Store_Key[i]].Item_List[j] == Store_Information.ItemType.car || Store_Dic[Store_Key[i]].Item_List[j] == Store_Information.ItemType.removead)
                {
                    information_List[j].transform.GetComponent<Text>().text = MyLocalization.Exchange(Store_Dic[Store_Key[i]].Value_List[j]);
                }
                else
                {
                    information_List[j].transform.GetComponent<Text>().text = MyLocalization.Exchange(string.Format("{0}_", Store_Dic[Store_Key[i]].Item_List[j].ToString()), Store_Dic[Store_Key[i]].Value_List[j]);
                }
            }
            else
            {
                information_List[j].gameObject.SetActive(false);
                information_List[j].transform.GetComponent<Text>().text = "";
            }
        }

        select_Item.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void Click_Category()
    {
        Setting_Category(EventSystem.current.currentSelectedGameObject.name);
        // 카테고리별 아이탬 변경
    }

    private void Setting_Category(string category_name)
    {
        for (int i = 0; i < Store_Key.Count; i++)
        {
            if (Item_Parents.transform.childCount != 0)
            {
                DestroyImmediate(Item_Parents.transform.GetChild(0).gameObject);
            }
        }
        Select_Item_List.Clear();
        //int first = Store_Key.Count;

        List<Store_Information> iList = new List<Store_Information>();

        foreach (var v in Store_Dic.Values)
        {
            if (v.Category == category_name)
            {
                if (!iList.Contains(v))
                {
                    iList.Add(v);
                }
            }
        }

        foreach (var t in iList)
        {
            if(!t.IsPurchased)
            {
                item = Instantiate(Item, new Vector2(0, 0), Quaternion.identity);
                item.transform.SetParent(Item_Parents.transform, false);
                item.name = t.Item_Name;

                if (t.Category == "Package")
                {
                    item.transform.GetChild(0).GetComponent<Text>().text = MyLocalization.Exchange(t.Item_Name);
                }
                else
                {
                    if (t.Item_List[0] == Store_Information.ItemType.car)
                    {
                        item.transform.GetChild(0).GetComponent<Text>().text = MyLocalization.Exchange(t.Value_List[0]);
                    }
                    else
                    {
                        item.transform.GetChild(0).GetComponent<Text>().text = MyLocalization.Exchange(string.Format("{0}_", t.Item_List[0].ToString()), t.Value_List[0]);
                    }
                }

                //item.transform.GetChild(0).GetComponent<Text>().text = MyLocalization.Exchange(t.Item_Name);
                item.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("image/{0}", t.Item_Name));
                item.transform.GetChild(2).GetComponent<Text>().text = t.Price;

                if (t.Price_Type == WealthManager.WealthType.RealMoney)
                {
                    item.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 80, 0);
                    item.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    item.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector3(50, 80, 0);
                    item.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                }

                //Debug.Log(t);
                Select_Item_List.Add(item);
            }
        }
        first_item = Store_Key.Count;
        for (int i = 0; i < Store_Key.Count; i++)
        {
            Reset_Item(i, category_name);
        }
        Item_Parents.transform.Reset();
    }

    public void Click_Item()
    {
        for (int i = 0; i < Store_Key.Count; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.name == Store_Dic[Store_Key[i]].Item_Name)
            {
                select_Item.transform.GetChild(3).gameObject.SetActive(false);
                select_Item = EventSystem.current.currentSelectedGameObject;
                item_Text(i);
                current_num = i;
                Select_Item_Image.sprite = select_Item.transform.GetChild(1).GetComponent<Image>().sprite;
                Buy_Btn_Text.text = Store_Dic[Store_Key[i]].Price + "";

                Buy_Image_Active(i);
            }
        }

        // 현재 선택한 아이탬
        // 선택한 아이탬 이미지 / 텍스트 변경
    }

    private void Buy_Image_Active(int i)
    {
        if (Store_Dic[Store_Key[i]].Price_Type == WealthManager.WealthType.RealMoney)
        {
            Buy_Image.SetActive(false);
            Buy_Btn_Text.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        }
        else
        {
            Buy_Btn_Text.GetComponent<RectTransform>().anchoredPosition = new Vector3(70, 0, 0);
            Buy_Image.SetActive(true);
        }
    }


    public void Back_Btn()
    {
        Menu_UI.gameObject.SetActive(true);
        Store_UI.gameObject.SetActive(false);
        // 취소 버튼
    }

    public void Buy_Btn()
    {
        Game_Manager.Instance.iapmanager.PurchaseProduct(Store_Dic[Store_Key[current_num]]);

        //Game_Manager.Instance.iapmanager.PurchaseProduct(Store_Dic[select_Item.name]);
        Setting_Category(Store_Dic[Store_Key[current_num]].Category);
    }

public void Click_Store() // Click_Store
    {
        Menu_UI.gameObject.SetActive(false);
        Store_UI.gameObject.SetActive(true);
        Category_Parents.transform.Reset();
        Setting_Category("StarShop");
        //상점 누를때 마다 상점 위치 리셋
    }
}