using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System;

namespace BE
{
    public class BackendManager : MonoBehaviour
    {
        //private BackendReturnObject bro = new BackendReturnObject();

        [SerializeField]
        private Text loadingtext = null;
        private const string loadingString = "Loading. . .({0}%)";

        private bool isInitialized;
        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }
        
        public Dictionary<WealthManager.WealthType, int> Wealth;
        
        private void Start()
        {
            //isGettingRanking = false;

            Backend.Initialize(() =>
            {
                // 초기화 성공한 경우 실행
                if (Backend.IsInitialized)
                {
                    isInitialized = true;

                    StartCoroutine(InitBegin());
                }
                // 초기화 실패한 경우 실행 
                else
                {
                    isInitialized = false;
                }
            });
            
        }

        private IEnumerator InitBegin()
        {
            loadingtext.text = string.Format(loadingString, 0);
            yield return null;

            BackeEndTokenLogIn();
            //loadingtext.text = string.Format(loadingString,10);
            yield return null;

            GetPlayerInfo();
            loadingtext.text = string.Format(loadingString, 20);
            yield return null;

            GetPlayerStat();
            loadingtext.text = string.Format(loadingString, 30);
            yield return null;

            WelcomeNewUser();
            loadingtext.text = string.Format(loadingString, 40);
            yield return null;

            GetFreshCarStat();
            loadingtext.text = string.Format(loadingString, 50);
            yield return null;

            GetPersonalRecord();
            loadingtext.text = string.Format(loadingString, 60);
            yield return null;

            //StartCoroutine(GetWholeRecord());
            GetWholeRecord();
            loadingtext.text = string.Format(loadingString, 70);
            yield return null;

            GetProductList();
            loadingtext.text = string.Format(loadingString, 80);
            yield return null;

            GetCash();
            loadingtext.text = string.Format(loadingString, 90);
            yield return null;

            Game_Manager.Instance.Initialize(loadingtext);
            loadingtext.text = string.Format(loadingString, 100);
            yield return null;
        }

        private Dictionary<string, UIDInfo> uUIDs;
        public struct UIDInfo
        {
            public string name;
            public string uuid;
            public int price;
        }

        public void GetCash()
        {
            if (Wealth == null)
            {
                Wealth = new Dictionary<WealthManager.WealthType, int>();
            }

            BackendReturnObject bro = Backend.TBC.GetTBC();

            var s = bro.GetReturnValuetoJSON()[0];

            if (Wealth.ContainsKey(WealthManager.WealthType.Cash))
            {
                Wealth[WealthManager.WealthType.Cash] = Convert.ToInt32(s.ToString());
            }
            else
            {
                Wealth.Add(WealthManager.WealthType.Cash, Convert.ToInt32(s.ToString()));
            }

            if (Wealth.Count > 1)
            {
                WealthManager.Instance.DIsplayWealth(Wealth);
            }
        }
        
        private void GetProductList()
        {
            uUIDs = new Dictionary<string, UIDInfo>();

            BackendReturnObject bro = Backend.TBC.GetProductList();

            var r = bro.GetReturnValuetoJSON()["rows"];

            LitJson.JsonData[] rows = new LitJson.JsonData[r.Count];
            
            for (int i =0; i< rows.Length; i++)
            {
                rows[i] = r[i];
            }
            
            foreach (var w in rows)
            {
                UIDInfo u = new UIDInfo();
                
                u.name = w["name"][0].ToString();
                u.uuid = w["uuid"][0].ToString();
                u.price = Convert.ToInt32(w["TBC"][0].ToString());

                uUIDs.Add(u.name, u);
            }
        }

        public UIDInfo GetProductUUID(string key)
        {
            if (uUIDs.ContainsKey(key))
            {
                return uUIDs[key];
            }
            else
            {
                UIDInfo u = new UIDInfo();

                u.name = "";
                u.price = 0;

                return u;
            }
        }

        public void GetPersonalRecord()
        {
            DesignConstStorage.PersonalTrackRecord = new Dictionary<string, float>();

            foreach (var r in DesignConstStorage.TrackDataTable.Rows)
            {
                DesignConstStorage.PersonalTrackRecord.Add(r.Get<string>("index"), DesignConstStorage.defaultRecord);
            }

            BackendReturnObject bro = Backend.GameInfo.GetPrivateContents(DesignConstStorage.tNameTrackRecord);

            var tRecord = bro.GetReturnValuetoJSON()[0][0];
            
            foreach (var key in tRecord.Keys)
            {
                string k = key;

                switch (k)
                {
                    case "inDate":
                        PInfo.trackinDate = tRecord[k][0].ToString();
                        break;
                    case "client_date":
                    case "updatedAt":
                        break;
                    default:
                        DesignConstStorage.PersonalTrackRecord[k] = (float)Convert.ToDouble(tRecord[k][0].ToString());

                        break;
                };
            }
        }
        
        //private bool isGettingRanking = false;

        public Dictionary<string, List<PlayerRankData>> TRecord;
       
        //public IEnumerator GetWholeRecord()
        //{
        //    //비동기 형식으로 진행한다
        //    if (isGettingRanking)
        //    {
        //        Debug.LogError("is on Getting Ranking?");
        //        yield return null;
        //    }
        //    else
        //    {
        //        isGettingRanking = true;

        //        if (rankingList == null)
        //        {
        //            Backend.Rank.RankList((callback) => {
        //                SetWholeRankings(callback);
        //            });
        //        }

        //        yield return new WaitUntil(() => rankingList != null);

        //        if (TRecord == null)
        //        {
        //            TRecord = new Dictionary<string, List<PlayerRankData>>();
        //        }

        //        foreach (var info in rankingList)
        //        {
        //            GetRanking(info.Value);
        //        }

        //        //isGettingRanking = false;
        //    }
        //}

        public void GetWholeRecord()
        {
            BackendReturnObject bro = Backend.Rank.RankList();

            Dictionary<string, RankingInfo> rankingList = new Dictionary<string, RankingInfo>();

            var j = bro.GetReturnValuetoJSON()["rows"];

            LitJson.JsonData[] rows = new LitJson.JsonData[j.Count];

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = j[i];
            }

            foreach(var row in rows)
            {
                RankingInfo rInfo = new RankingInfo();

                foreach (var k in row.Keys)
                {
                    switch (k)
                    {
                        case "date":
                            rInfo.dateType = (RankingInfo.Date)Enum.Parse(typeof(RankingInfo.Date), row[k][0].ToString());
                            break;
                        case "range":
                            rInfo.range = row[k][0].ToString();
                            break;
                        case "inDate":
                            rInfo.inDate = row[k][0].ToString();
                            break;
                        case "uuid":
                            rInfo.UUID = row[k][0].ToString();
                            break;
                        case "title":
                            rInfo.rankingName = row[k][0].ToString();
                            break;
                    }

                    //Debug.LogError(string.Format("{0} {1} {2}",e[0], e[1], e[2]));
                }

                rankingList.Add(rInfo.rankingName, rInfo);
            }
            
            if (TRecord == null)
            {
                TRecord = new Dictionary<string, List<PlayerRankData>>();
            }

            foreach (var info in rankingList)
            {
                GetRanking(info.Value);
            }
        }

        private void GetRanking(RankingInfo info)
        {
            BackendReturnObject bro = Backend.Rank.GetRankByUuid(info.UUID, DesignConstStorage.defaultRankAmount);

            var rankData = bro.GetReturnValuetoJSON()["rows"];

            List<PlayerRankData> rData = new List<PlayerRankData>();

            LitJson.JsonData[] rows = new LitJson.JsonData[rankData.Count];

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = rankData[i];
            }

            foreach (var pData in rows)
            {
                PlayerRankData prd = new PlayerRankData();
                foreach (var k in pData.Keys)
                {
                    switch (k)
                    {
                        case "nickname":
                            prd.nickName = pData[k][0].ToString();
                            break;
                        case "score":
                            prd.score = (float)Convert.ToDouble(pData[k][0].ToString());
                            break;
                        case "rank":
                            prd.rank = Convert.ToInt32(pData[k][0].ToString());
                            break;
                        case "index":
                            prd.index = Convert.ToInt32(pData[k][0].ToString());
                            break;
                        case "gamerIndate":
                            prd.playerinDate = pData[k][0].ToString();
                            break;
                        default:
                            Debug.LogError(pData[k][0].ToString());
                            break;
                    }

                    //Debug.LogError(string.Format("{0} {1} {2}",e[0], e[1], e[2]));
                }
                rData.Add(prd);
            }

            if (TRecord.ContainsKey(info.range))
            {
                TRecord.Remove(info.range);
            }

            TRecord.Add(info.range, rData);

            //ProcessRankData(bro, info);
        }

        private void ProcessRankData(BackendReturnObject bro, RankingInfo info)
        {
            JObject rankData = JObject.Parse(bro.GetReturnValue());

            List<PlayerRankData> rData = new List<PlayerRankData>();

            //다른 프로퍼티 조회해서 개수 구할 수도 있을듯
            var data = rankData["rows"];

            JArray playerDatas = JArray.Parse(data.ToString());
            foreach (var pData in playerDatas)
            {
                PlayerRankData prd = new PlayerRankData();
                foreach (var element in pData)
                {
                    string[] splitedelement = element.ToString().Split('"');

                    //string[] e = { splitedelement[1], splitedelement[3], splitedelement[5] };
                    string t;
                    string[] temp;
                    switch (splitedelement[1])
                    {
                        case "nickname":
                            prd.nickName = splitedelement[5];
                            break;
                        case "score":
                            t = splitedelement[4].Replace("\r", ":");
                            temp = t.Split(':');

                            prd.score = (float)Convert.ToDouble(temp[1]);
                            break;
                        case "rank":
                            t = splitedelement[4].Replace("\r", ":");
                            temp = t.Split(':');

                            prd.rank = Convert.ToInt32(temp[1]);
                            break;
                        case "index":
                            t = splitedelement[4].Replace("\r", ":");
                            temp = t.Split(':');

                            prd.index = Convert.ToInt32(temp[1]);
                            break;
                        case "gamerIndate":
                            prd.playerinDate = splitedelement[5];
                            break;
                        default:
                            Debug.LogError(splitedelement[1]);
                            break;
                    }

                    //Debug.LogError(string.Format("{0} {1} {2}",e[0], e[1], e[2]));
                }
                rData.Add(prd);
            }

            if (TRecord.ContainsKey(info.range))
            {
                TRecord.Remove(info.range);
            }

            TRecord.Add(info.range, rData);
            //Debug.LogError(TRecord[info.range].Count);

            //isGettingRanking = false;
        }

        public class PlayerRankData
        {
            public string nickName = "";
            public float score;
            public int index;
            public string playerinDate;
            public int rank;
        }
        
        public class RankingInfo
        {
            public string rankingName;
            public string UUID;
            public string inDate;
            public enum Date
            {
                day,
                week,
                month,
                all
            }

            public Date dateType = Date.all;

            //얘가 왜 트랙 이름이냐
            public string range;
        }
        
        public void GetPlayerStat()
        {
            
            //동기진행한다.

            BackendReturnObject returnObject = Backend.GameInfo.GetTableList();

            DesignConstStorage.PrivateTableNameList = new List<string>();
            DesignConstStorage.PublicTableNameList = new List<string>();

            string ptName = "privateTables";

            JObject obj = JObject.Parse(returnObject.GetReturnValue());

            List<string> l;
            //Foreach로 각 테이블 이름 받아옴.
            foreach (var a in obj)
            {
                if (a.Key == ptName)
                {
                    l = DesignConstStorage.PrivateTableNameList;
                }
                else
                {
                    l = DesignConstStorage.PublicTableNameList;
                }
                JArray array = JArray.Parse(a.Value.ToString());
                foreach (var s in array)
                {
                    l.Add(s.ToString());
                }
            }
            
        }
        
        public class PlayerInfo
        {
            public string nickname;
            public string inDate;
            public string subscriptionType;
            public string emailForFindPassword;
            public string wealthinDate;
            public string carinDate;
            public string trackinDate;
        }

        public PlayerInfo PInfo;

        private void GetPlayerInfo()
        {
            BackendReturnObject bro = Backend.BMember.GetUserInfo();

            PInfo = new PlayerInfo();
            
            var p = bro.GetReturnValuetoJSON()[0];

            Dictionary<string, string> pData = new Dictionary<string, string>();

            foreach (var d in p)
            {
                string nS = d.ToString().Replace("[", "");

                nS = nS.Replace("]", "");

                string[] s = nS.Split(',');

                switch (s[0])
                {
                    case "nickname":
                        PInfo.nickname = s[1];
                        break;
                    case "inDate":
                        PInfo.inDate = s[1];
                        break;
                    case "subscriptionType":
                        PInfo.subscriptionType = s[1];
                        break;
                    case "emailForFindPassword":
                        PInfo.emailForFindPassword = s[1];
                        break;
                    default:
                        Debug.LogError("Wrong Value");
                        break;
                }
            }

            PInfo.wealthinDate = "";
        }
        
        public void BackeEndTokenLogIn()
        {
            //동기화 되어야 하는 기능

            BackendReturnObject returnObject;

            if (PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID) == "")
            {
                //ID값이 초기값이라고?? 그럼 회원가입 해야지
                SignUp();
                return;
            }
            else
            {
                returnObject = Backend.BMember.LoginWithTheBackendToken();
            }

            string sCode = returnObject.GetStatusCode();

            if(sCode == "403")
            {
                ShowBlockUserInfo();

                return;
            }

            if (returnObject.IsSuccess())
            {
                return;
            }

            string asd = PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID);
            Debug.LogError(PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID));

            //초기값 아니니까 기존 ID PW로 로그인 ㄱㄱ

            BackendReturnObject cloginBRO = Backend.BMember.CustomLogin(PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID), (PlayerPrefs.GetString(DesignConstStorage.PlayerCustomPW)));

            string clSCode = cloginBRO.GetStatusCode();

            switch (clSCode)
            {
                case "200":
                case "201":
                case "204":
                    //성공
                    //GetPlayerInfo();
                    break;
                case "410":
                    //아디 없음
                    //회원가입 하면 된다?
                    //테스트 상황에서는 여기  문제가 된다. 임시로 회원가입
                    Debug.LogWarning("Waring!!");
                    SignUp();
                    break;
                case "409":
                    //아디 중복
                    //로그인시 409에러면 이상할거 같은데?
                    Debug.LogError("Wrong Value");
                    break;
                case "403":
                    ShowBlockUserInfo();
                    break;
                case "401":
                    //비번 틀림
                    //저장된 비번이 달라졌으니까 가능성은 2개다.
                    //1 해킹이나 뭐든간에 변조
                    //2 로그인시 잘못된 아디값으로 로그인
                    //테스트 상황에서는 여기  문제가 된다. 임시로 회원가입
                    Debug.LogWarning("Waring!!");
                    SignUp();
                    break;
                default:
                    Debug.LogError(clSCode);
                    break;
            }

        }

        public void ChangeLogInType()
        {
            //https://developer.thebackend.io/unity3d/guide/bmember/signup_login/
            //참조해서 아이디 로그인 방식 변환한다.
            DesignConstStorage.DoItList();
            
        }

        private void ShowBlockUserInfo()
        {
            Debug.LogWarning("당신은 차단당했습니다. 파하하");
        }

        private void GenIDPW()
        {
            Debug.Log("IDPW Gen");

            PlayerPrefs.SetString(DesignConstStorage.PlayerCustomID, RandomizeString());
            PlayerPrefs.SetString(DesignConstStorage.PlayerCustomPW, RandomizeString());
        }

        private string RandomizeString()
        {
            char[] c = new char[16];

            for (int i = 0; i < c.Length; i++)
            {
                c[i] = RandomizeChar();
            }

            return string.Format("{0}{1}{2}{3}_{4}{5}{6}{7}_{8}{9}{10}{11}_{12}{13}{14}{15}", c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7], c[8], c[9], c[10], c[11], c[12], c[13], c[14], c[15]);
        }

        private char RandomizeChar()
        {
            char c;

            int min;
            int max;

            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    min = 48;
                    max = 58;
                    break;
                case 1:
                    min = 65;
                    max = 91;
                    break;
                case 2:
                    min = 97;
                    max = 123;
                    break;
                default:
                    //0,1,2 아닌데 들오면 에러지
                    min = 0;
                    max = 1;
                    Debug.LogError("Wrong");
                    break;
            }

            c = (char)UnityEngine.Random.Range(min, max);

            while (!(c != 'I' && c != 'l'))
            {
                c = (char)UnityEngine.Random.Range(min, max);
            }
            
            return c;
        }

        private bool welcomeFalg = false;

        public void SignUp()
        {
            //BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
            GenIDPW();

            BackendReturnObject returnObject = Backend.BMember.CustomSignUp(PlayerPrefs.GetString(DesignConstStorage.PlayerCustomID), (PlayerPrefs.GetString(DesignConstStorage.PlayerCustomPW)));
            
            string sCode = returnObject.GetStatusCode();

            switch (sCode)
            {
                case "409":
                    SignUp();
                    break;
                case "401":
                    //회원가입 하는데 비번이 틀림????
                    //리얼리??????????
                    Debug.LogError(sCode);
                    break;
                case "403":
                    ShowBlockUserInfo();
                    break;

                case "201":
                    welcomeFalg = true;
                    //WelcomeNewUser();

                    break;

                default:
                    
                    break;
            }

            loadingtext.text = returnObject.GetMessage();
        }

        public void WelcomeNewUser()
        {
            if (!welcomeFalg)
            {
                return;
            }

            AddNewCar(DesignConstStorage.CarDataTable.Rows[0].Get<string>("index"), true);

            Param param = new Param();

            int initGM = 50;

#if UNITY_EDITOR

            initGM = 500;

#endif

            param.Add(WealthManager.WealthType.GameMoney.ToString(), initGM);
            //param.Add(WealthManager.WealthType.Cash.ToString(), 0);
            BackendReturnObject bro = Backend.GameInfo.Insert(DesignConstStorage.tNameWealth, param);

            param = new Param();

            param.Add(DesignConstStorage.TrackDataTable.Rows[0].Get<string>("index"), DesignConstStorage.defaultRecord);

            bro = Backend.GameInfo.Insert(DesignConstStorage.tNameTrackRecord, param);
        }
        
        private const string carSizeString = "Size";

        public void AddNewCar(string car, bool flag = false)
        {
            Param param = new Param();

            //param.Add("index", car);

            Dictionary<string, int> stat = new Dictionary<string, int>();
            
            for (int i = 0; i < DesignConstStorage.CarStatDoneName.Length; i++)
            {
                stat.Add(DesignConstStorage.CarStatDoneName[i], 0);
            }

            stat.Add(carSizeString, (int)Car_Information.CarSize.Normal);

            param.Add(car, stat);

            BackendReturnObject bro;

            if (flag)
            {
                bro = Backend.GameInfo.Insert(DesignConstStorage.tNameCarStat, param);
            }
            else
            {
                bro = Backend.GameInfo.Update(DesignConstStorage.tNameCarStat, PInfo.carinDate, param);
            }
        }

        public void CalcWealth(WealthManager.WealthType type, GameInfoOperator op,int amount)
        {
            Param param = new Param();

            param.AddCalculation(type.ToString(), op, amount);
            
            BackendReturnObject bro = Backend.GameInfo.UpdateWithCalculation(DesignConstStorage.tNameWealth, PInfo.wealthinDate, param);

            string sCode = bro.GetStatusCode();

            if (sCode != "204")
            {
                Debug.LogError(sCode);
            }
            else
            {
            }
        }

        public void IncreaseStat(Car_Information car, DesignConstStorage.StatList stat,int increaseAmount = 1)
        {
            string cName = car.CarName;

            GetFreshCarStat();

            car = Game_Manager.Instance.CarStat[cName];
            car.IncreaseStat(stat, increaseAmount);

            Param param = new Param();

            //param.AddCalculation(car.CarName, GameInfoOperator.addition, increaseAmount);

            //BackendReturnObject bro = Backend.GameInfo.UpdateWithCalculation(DesignConstStorage.tNameCarStat, PInfo.carinDate, param);

            Dictionary<string, int> status = new Dictionary<string, int>();

            status.Add(DesignConstStorage.CarStatDoneName[0], car.TSD);
            status.Add(DesignConstStorage.CarStatDoneName[1], car.BRD);
            status.Add(DesignConstStorage.CarStatDoneName[2], car.COD);
            status.Add(DesignConstStorage.CarStatDoneName[3], car.ACD);

            param.Add(car.CarName, status);
            BackendReturnObject bro = Backend.GameInfo.Update(DesignConstStorage.tNameCarStat, PInfo.carinDate, param);
            
            if (bro.IsSuccess())
            {
#if UNITY_EDITOR
                int temp = 0;
                switch (stat)
                {
                    case DesignConstStorage.StatList.accelerating:
                        temp = car.ACD;
                        break;
                    case DesignConstStorage.StatList.brake:
                        temp = car.BRD;
                        break;
                    case DesignConstStorage.StatList.cornering:
                        temp = car.COD;
                        break;
                    case DesignConstStorage.StatList.topspeed:
                        temp = car.TSD;
                        break;
                }


                //Debug.Log("업그레이드 성공 !! 현재 업그레이드 횟수 :  " + temp);
#endif
            }
            else
            {
                string sCode = bro.GetStatusCode();
                //Debug.LogError(sCode);
            }
        }

        public void SetPersonalRecordToServer(string tName)
        {
            //삽입인지 바로 넣을지 결정해야 한다.


            Param param = new Param();
            
            param.Add(tName, DesignConstStorage.PersonalTrackRecord[tName]);
            
            BackendReturnObject bro = Backend.GameInfo.Update(DesignConstStorage.tNameTrackRecord, PInfo.trackinDate, param);

            if (bro.IsSuccess())
            {
#if UNITY_EDITOR
                Debug.LogError(bro.IsSuccess());
                Debug.LogError("더 느린 기록이라도 업데이트 되는지 확인");
#endif
            }
            else
            {
                Debug.LogError(bro.GetStatusCode());
            }
        }
        
        public void GetFreshCarStat()
        {
            BackendReturnObject bro = Backend.GameInfo.GetPrivateContents(DesignConstStorage.tNameCarStat);
            
            var cStat = bro.GetReturnValuetoJSON()[0][0];

            Dictionary<string, Car_Information> cData = new Dictionary<string, Car_Information>();

            foreach (var key in cStat.Keys)
            {
                string k = key;

                switch (k)
                {
                    case "inDate":
                        PInfo.carinDate = cStat[k][0].ToString();
                        break;
                    case "client_date":
                    case "updatedAt":
                        break;
                    default:
                        Car_Information cInfo = new Car_Information();

                        var stat = cStat[k][0];

                        cInfo.CarName = k;

                        foreach (var sKey in stat.Keys)
                        {
                            //Debug.LogError(string.Format("{0}  {1}", sKey, stat[sKey][0].ToString()));

                            //cInfo.CarName = sKey;
                            DesignConstStorage.StatList s = DesignConstStorage.StatList.brake; 

                            switch (sKey)
                            {
                                case "TSD":
                                    s = DesignConstStorage.StatList.topspeed;
                                    break;
                                case "ACD":
                                    s = DesignConstStorage.StatList.accelerating;
                                    break;
                                case "COD":
                                    s = DesignConstStorage.StatList.cornering;
                                    break;
                                case "BRD":
                                    s = DesignConstStorage.StatList.brake;
                                    break;
                                case carSizeString:
                                    cInfo.SetSize(stat[sKey][0].ToString());
                                    continue;
                            }

                            cInfo.SetDoneInfo(s, Convert.ToInt32(stat[sKey][0].ToString()));
                        }

                        cInfo.SetPurchase();

                        cData.Add(k, cInfo);
                        break;
                };
            }

            foreach (var car in DesignConstStorage.CarDataTable.Rows)
            {
                string carName = car.Get<string>("index");

                if (!cData.ContainsKey(carName))
                {
                    Car_Information cInfo = new Car_Information();

                    cInfo.CarName = carName;
                    cInfo.SetDoneInfo(DesignConstStorage.StatList.accelerating);
                    cInfo.SetDoneInfo(DesignConstStorage.StatList.brake);
                    cInfo.SetDoneInfo(DesignConstStorage.StatList.cornering);
                    cInfo.SetDoneInfo(DesignConstStorage.StatList.topspeed);

                    cInfo.SetPurchase(false);

                    cData.Add(carName, cInfo);
                }
            }

            Game_Manager.Instance.CarStat = cData;
        }

        public void GetFreshWealthInfo()
        {
            //Dictionary<WealthManager.WealthType, int> freshWealth = new Dictionary<WealthManager.WealthType, int>();

            if(Wealth == null)
            {
                Wealth = new Dictionary<WealthManager.WealthType, int>();
            }
            
            BackendReturnObject bro = Backend.GameInfo.GetPrivateContents(DesignConstStorage.tNameWealth);

            var wTable = bro.GetReturnValuetoJSON()[0][0];

            //foreach (var t in wTable)
            //{
            //}
            
            Dictionary<string, string> wData = new Dictionary<string, string>();

            foreach (var key in wTable.Keys)
            {
                string k = key;
                string v = wTable[k][0].ToString();

                wData.Add(k, v);
            }

            PInfo.wealthinDate = wData["inDate"];

            if (Wealth.ContainsKey(WealthManager.WealthType.GameMoney))
            {
                Wealth[WealthManager.WealthType.GameMoney] = Convert.ToInt32(wData[WealthManager.WealthType.GameMoney.ToString()]);
            }
            else
            {
                Wealth.Add(WealthManager.WealthType.GameMoney, Convert.ToInt32(wData[WealthManager.WealthType.GameMoney.ToString()]));
            }

            if(Wealth.Count > 1)
            {
                WealthManager.Instance.DIsplayWealth(Wealth);
            }

            //현금 타입은 저장할 수 없지 암..
            //for (int i = 0; i < ((int)WealthManager.WealthType.RealMoney); i++)
            //{
            //    freshWealth.Add((WealthManager.WealthType)i, Convert.ToInt32(wData[((WealthManager.WealthType)i).ToString()]));
            //}
            
            //Wealth = freshWealth;
        }
        
        private void wwww(string s)
        {
            //요 콜백 두번씩 들어오는데???
            //뒤끝에 최무니 한번 해보자
        }

        public string ServerTimer()
        {
            Backend.Utils.GetServerTime((callback) => {
                // 이후 처리
                wwww(callback.GetReturnValue());
            });

            if (Backend.Utils != null)
            {
                return Backend.Utils.GetServerTime().GetReturnValue();
            }
            else
            {
                return "";
            }
        }
    }
}