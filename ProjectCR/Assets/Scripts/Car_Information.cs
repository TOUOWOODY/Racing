using UnityEngine;

public class Car_Information
{
    public string CarName;

    private int tsD;
    public int TSD
    {
        get
        {
            return tsD;
        }
    }

    private int acD;
    public int ACD
    {
        get
        {
            return acD;
        }
    }

    private int coD;
    public int COD
    {
        get
        {
            return coD;
        }
    }

    private int brD;
    public int BRD
    {
        get
        {
            return brD;
        }
    }

    private bool isPurchased = false;
    public bool IsPurchased
    {
        get
        {
            return isPurchased;
        }
    }

    private CarSize size = CarSize.Normal;
    public CarSize Size
    {
        get
        {
            return size;
        }
    }

    public void SetPurchase(bool b = true)
    {
        isPurchased = b;
    }
    
    public enum CarSize
    {
        Big,
        Normal,
        Compact,
        Small
    }

    public void SetDoneInfo(DesignConstStorage.StatList stat, int i = 0)
    {
        switch (stat)
        {
            case DesignConstStorage.StatList.accelerating:
                acD = i;
                break;
            case DesignConstStorage.StatList.brake:
                brD = i;
                break;
            case DesignConstStorage.StatList.cornering:
                coD = i;
                break;
            case DesignConstStorage.StatList.topspeed:
                tsD = i;
                break;
        }
    }

    public void SetSize(string s)
    {
        size = (CarSize)System.Convert.ToInt32(s);
        //Debug.LogError(size);
    }

    public void IncreaseStat(DesignConstStorage.StatList stat, int i)
    {
        switch (stat)
        {
            case DesignConstStorage.StatList.accelerating:
                acD += i;
                break;
            case DesignConstStorage.StatList.brake:
                brD += i;
                break;
            case DesignConstStorage.StatList.cornering:
                coD += i;
                break;
            case DesignConstStorage.StatList.topspeed:
                tsD += i;
                break;
        }
    }
}