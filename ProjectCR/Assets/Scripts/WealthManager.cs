using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WealthManager : MonoBehaviour
{

    [SerializeField]
    private Money_Manager money_Manager;

    private static WealthManager instance = null;
    public static WealthManager Instance
    {
        get
        {
            return instance;
        }
    }

    public enum WealthType
    {
        GameMoney,
        Cash,
        RealMoney
    }

    private void RefreshWealth()
    {
        Game_Manager.Instance.backendManager.GetFreshWealthInfo();
    }

    public bool IncomeWealth(WealthType type, int amount)
    {
        Game_Manager.Instance.backendManager.CalcWealth(type, BackEnd.GameInfoOperator.addition, amount);

        Game_Manager.Instance.backendManager.GetFreshWealthInfo();

        return false;
    }

    public bool SpendWealth(WealthType type, int amount)
    {
        /*
         * 1차로 클라이언트 데이터로 검증한다.
         * 2차로 서버 데이터를 받아와서 검증한다.
         * 둘 다 통과되면 계산한다.
         */
        if (Game_Manager.Instance.backendManager.Wealth[type] >= amount)
        {
            Game_Manager.Instance.backendManager.GetFreshWealthInfo();

            if (Game_Manager.Instance.backendManager.Wealth[type] >= amount)
            {
                Game_Manager.Instance.backendManager.CalcWealth(type, BackEnd.GameInfoOperator.subtraction, amount);
                Game_Manager.Instance.backendManager.Wealth[type] -= amount;

                DIsplayWealth(Game_Manager.Instance.backendManager.Wealth);
                return true;
            }
        }
        
        Debug.LogError("돈이 부족합니다.");
        return false;
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        RefreshWealth();
    }

    private bool CheckWealth(bool b)
    {

        return b;
    }
    
    public void DIsplayWealth(Dictionary<WealthType, int> d)
    {
        money_Manager.Display_Wealth(d[(WealthType)0].ToString(), d[(WealthType)1].ToString());
    }

    public int Get_Money(int i)
    {

        return 1;
    }
}