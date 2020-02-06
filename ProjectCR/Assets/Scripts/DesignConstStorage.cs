using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DesignConstStorage
{
    public enum StatList
    {
        //추가된다면 For문이 달라지는 경우도 같이 수정해야 한다.
        topspeed,
        cornering,
        brake,
        accelerating
    }

    public const string tNameCarData = "CarData";
    public const string tNameCarStat = "CarStat";
    public const string tNameWealth = "Wealth";
    public const string tNameStore = "Store";
    public const string tNameTrackRecord = "TrackRecord";
    public const string tNameTrackData = "TrackData";
    public const string tNameCarPrice = "CarPrice";
    public const string tNameLocalize = "localize";

    public static readonly string[] CarStatDoneName = { "TSD", "BRD", "COD", "ACD" };

    public const float defaultRecord = 59.999f;

    public const int defaultRankAmount = 10;

    //public const string ResourcePathFormat = "Resources/{0}";

    public const int MaxUpgrade = 10;
    
    //가, 감속시 최소값 보정치
    public const float AccModifier = 0.01f;
    public const float DecModifier = 0.015f;

    //가, 감속시 가중치
    public const float DecelerationFactor = 0.36f;
    public const float AccelerationFactor = 0.5f;

    //데미지 커브 계수
    public const float cDamageFactor = 1.53f;

    public const int SectionSize = 200;
    public const float MovingUnit = 0.01f;
    public const float DetectionRange = 0.2f;

    public const string PlayerCustomID = "CRID";
    public const string PlayerCustomPW = "CRPW";

    public const string sLine = "StartLine";
    public const string fLine = "FinishLine";

    public const float BoosterTime = 1.0f;

    public static void DoItList()
    {
        bool b = false;

        if (!b)
        {
            return;
        }
        

        DesignConstStorage.DoItList();
    }

    private static List<string> tips;
    public static string Tip
    {
        get
        {
            if (tips == null)
            {
                tips = new List<string>();

                const string t = "tip";
                foreach (var item in LocalizeTable.Rows)
                {
                    string k = item.Get<string>("key");
                    string[] key = k.Split('_');
                    if (key[0] == t)
                    {
                        tips.Add(MyLocalization.Exchange(k));
                    }
                }
            }

            int r = UnityEngine.Random.Range(0, tips.Count);

            return tips[r];
        }
    }

    public const string RealMoneyCarName5500 = "car_04";
    public const string RealMoneyCarName27500 = "car_13";

    public static List<string> PrivateTableNameList;
    public static List<string> PublicTableNameList;

    public static Dictionary<string, float> PersonalTrackRecord;

    public static readonly int[] gameMoneyReward = { 20, 15, 12, 9, 5, 1 };

    //public const string ResourcesFolderPath = "Resources/Image/{0}";
    public const string ResourcesFolderPath = "Resources/Table/{0}";

    //차량의 오리지널 데이터. 이 항목은 읽기 전용.
    public static TableHandler.Table CarDataTable = TableHandler.Get(tNameCarData, TableHandler.SteamMode.Resource);
    public static TableHandler.Table TrackDataTable = TableHandler.Get(tNameTrackData, TableHandler.SteamMode.Resource);
    public static TableHandler.Table CarPriceTable = TableHandler.Get(tNameCarPrice, TableHandler.SteamMode.Resource);
    public static TableHandler.Table LocalizeTable = TableHandler.Get(tNameLocalize, TableHandler.SteamMode.Resource);
    public static TableHandler.Table StoreTable = TableHandler.Get(tNameStore, TableHandler.SteamMode.Resource);
    //유저가 플레이를 반영하여 업글된 데이터. 이 데이터는 읽고 쓰기가 가능하다.
    //이 항목은 게임의 전역에서 접근하여 최신 상태로 갱신해둔다.
    //public static TableHandler.Table carStat = TableHandler.Get(tNameCarStat, TableHandler.SteamMode.AppData);
}