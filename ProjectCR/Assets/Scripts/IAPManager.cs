using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class IAPManager : MonoBehaviour, IStoreListener
{
	public static IStoreController storeController = null;
    static List<string> sProductIds;
    
    [SerializeField] Text txtLog;

	void Awake()
	{
        if (storeController == null)
		{
            GetStoreTableData();
        }
    }

    public void GetStoreTableData()
    {
        sProductIds = new List<string>();// = new string[] { "test_cash_1000", "test_car_001" };

        foreach (var row in DesignConstStorage.StoreTable.Rows)
        {
            if (row.Get<string>("pricetype") == "realmoney")
            {
                sProductIds.Add(row.Get<string>("index"));
            }
        }

        InitStore();
    }

    void InitStore()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var p in sProductIds)
        {
            builder.AddProduct(p, ProductType.Consumable, new IDs { { p, GooglePlay.Name } });
        }

        //builder.AddProduct(sProductIds[0], ProductType.Consumable, new IDs { { sProductIds[0], GooglePlay.Name } });
        //      builder.AddProduct(sProductIds[1], ProductType.Consumable, new IDs { { sProductIds[1], GooglePlay.Name } });

        UnityPurchasing.Initialize(this, builder);
    }
    
	void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		storeController = controller;
        //txtLog.text = "결제 기능 초기화 완료";
        foreach (var item in storeController.products.all)
        {
            if (item.availableToPurchase)
            {
                //item.transactionID,
                //item.metadata.localizedTitle,
                //item.metadata.localizedDescription,
                //item.metadata.isoCurrencyCode,
                //Price.Add(item.definition.id, item.metadata.localizedPrice.ToString());
                //item.metadata.localizedPriceString,
                //item.transactionID,
                //item.receipt
                //Game_Manager.Instance.store_manager.Store_Dic[item.definition.id].Price = item.metadata.localizedPrice.ToString();
                //Debug.Log(item.definition.id + item.metadata.isoCurrencyCode + test.text);
            }
        }
    }

	void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
	{
		//txtLog.text = "OnInitializeFailed" + error;
		//Debug.Log(txtLog.text);
    }

    public void OnBtnPurchaseClicked(Store_Information item)
    {
        if (storeController == null)
        {
            //txtLog.text = "구매 실패 : 결제 기능 초기화 실패";
            //Debug.Log(txtLog.text);
        }
        else
        {
            sInfo = item;
            storeController.InitiatePurchase(item.Item_Name);
        }   
    }

    private Store_Information sInfo = null;

	PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
	{
        bool isSuccess = true;
#if UNITY_ANDROID && !UNITY_EDITOR
		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
		try
		{
			IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
			for(int i = 0; i < result.Length; i++)
				Analytics.Transaction(result[i].productID, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode, result[i].transactionID, null);
		}
		catch (IAPSecurityException)
		{
			isSuccess = false;
		}
#elif UNITY_IPHONE


#endif
        BackEnd.BackendReturnObject bro = null;

        if (isSuccess)
		{
            bool isTBCCarge = true;

            switch (e.purchasedProduct.definition.id)
            {
                case "car_5500":
                case "car_27500":
                    isTBCCarge = false;
                    break;

                default:
                    isTBCCarge = true;
                    break;
            }

#if UNITY_ANDROID && !UNITY_EDITOR

            
            if (isTBCCarge)
            {
                bro = BackEnd.Backend.TBC.ChargeTBC(e.purchasedProduct.receipt, "");
            }
            else
            {
                bro = BackEnd.Backend.Receipt.IsValidateGooglePurchase(e.purchasedProduct.receipt, "");
            }
                

            if (bro.IsSuccess())
            {

            }
            else
            {
                return PurchaseProcessingResult.Pending;
            }

            //var asd = bro.GetReturnValuetoJSON()[0];
            //var qwe = asd[0];

            //값 뭐 들어오나 확인해야 한
            DesignConstStorage.DoItList();

            //txtLog.text = string.Format("{1}", e.purchasedProduct.receipt, bro.GetReturnValue());
            //txtLog.text = string.Format("{0}", asd.ToString());
            //txtLog.text = string.Format("{0}  {1}", asd.ToString(), qwe.ToString());

            //string[] qweqwe = new string[2];
            //int qq = 0;
            
            //JObject obj = JObject.Parse(bro.GetReturnValue());

            //Foreach로 각 테이블 이름 받아옴.
            //foreach (var a in obj)
            //{
            //    string qwoitnwbye = string.Format("{0}  {1}", a.Key.ToString(), a.Value);
            //    qweqwe[qq++] = qwoitnwbye;
            //    txtLog.text = string.Format("{0}  {1}", a.Key.ToString(), a.Value);
            //    JArray array = JArray.Parse(a.Value.ToString());
            //    foreach (var s in array)
            //    {
            //        //txtLog.text = string.Format("{0}  {1}", , s.ToString());
            //        qweqwe[qq] = a.Key.ToString();
            //        int weqg = 
            //    }
            //}
            //BackEnd.Param param = new BackEnd.Param();
            //param.Add(bro.GetReturnValue());

            //BackEnd.Backend.GameInfo.InsertLog("1", param);

#elif UNITY_IPHONE
#else
            //bro = BackEnd.Backend.TBC.ChargeTBC(e.purchasedProduct.receipt, "");
#endif

            //if (bro != null)
            //{
            //}

            //여기서 현금 결제 아이템 제공한다.

            ProvideItems(e.purchasedProduct.definition.id);

            //Debug.Log("구매 완료");
            //         if (e.purchasedProduct.definition.id.Equals(sProductIds[0]))
            //             WealthManager.Instance.IncomeWealth(WealthManager.WealthType.Cash, 100);
            //else if (e.purchasedProduct.definition.id.Equals(sProductIds[1]))
            //             WealthManager.Instance.IncomeWealth(WealthManager.WealthType.Cash, 500);
        }
		else
		{
            //txtLog.text = "구매 실패 : 비정상 결제";
            //         Debug.Log(txtLog.text);
            //sInfo = null;
		}

        sInfo = null;
        return PurchaseProcessingResult.Complete;
	}

    private void ProvideItems(string ID)
    {
        if(sInfo!= null)
        {
            sInfo.ProvideItems();
        }
        
        sInfo = null;

        //switch(ID)
        //{
        //    case "car_5500":
        //        Game_Manager.Instance.backendManager.AddNewCar(DesignConstStorage.RealMoneyCarName5500);
        //        break;
        //    case "car_27500":
        //        Game_Manager.Instance.backendManager.AddNewCar(DesignConstStorage.RealMoneyCarName27500);
        //        break;
        //    default:
        //        Debug.LogError("Wrong Value");
        //        break;
        //}
    }

    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
	{
		if (!error.Equals(PurchaseFailureReason.UserCancelled))
		{
			//txtLog.text = "구매 실패 : " + error;
			//Debug.Log(txtLog.text);
		}
	}

    public bool PurchaseProduct(Store_Information p)
    {
        //상품의 결제 타입에 따라 각 위치에서 항목 제공한다.

        //Debug.LogError(p.Item_Name);
        //return false;

        switch (p.Price_Type)
        {
            case WealthManager.WealthType.Cash:
                //뒤끝쪽에 상품코드 전달한다.
                UseCash(p);
                break;
            case WealthManager.WealthType.GameMoney:
                //차량 구매 or 업글
                //게임머니 차감하고 차량 등록하거나 업글한다.
                TableHandler.Row row = TableHandler.Get(DesignConstStorage.tNameCarPrice, TableHandler.SteamMode.Resource).FindRow<string>("index", p.Item_Name);

                bool isPaid = WealthManager.Instance.SpendWealth(p.Price_Type, row.Get<int>("price") * 2);

                if (isPaid)
                {
                    p.ProvideItems();
                    //Debug.LogError(p.Item_Name);
                    //Game_Manager.Instance.backendManager.AddNewCar(p.Item_Name);
                    //Game_Manager.Instance.backendManager.GetFreshCarStat();
                }
                else
                {
                    return false;
                }
                break;
            case WealthManager.WealthType.RealMoney:
                OnBtnPurchaseClicked(p);
                break;
        }

        return true;
    }

    private void UseCash(Store_Information p)
    {
        BackEnd.BackendReturnObject bro = BackEnd.Backend.TBC.UseTBC(p.UUID, "");

        if (bro.IsSuccess())
        {
            p.ProvideItems();
            var s = bro.GetReturnValuetoJSON()["amountTBC"].ToString();
            Game_Manager.Instance.backendManager.Wealth[WealthManager.WealthType.Cash] = System.Convert.ToInt32(s);
            WealthManager.Instance.DIsplayWealth(Game_Manager.Instance.backendManager.Wealth);
        }
    }
}