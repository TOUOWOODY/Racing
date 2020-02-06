using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Status
{
    private int price;
    public int Price
    {
        get
        {
            return price;
        }
    }
    public int TopSpeed
    {
        get
        {
            return Status[DesignConstStorage.StatList.topspeed];
        }
    }

    public int TotalTS
    {
        get
        {
            return Status[DesignConstStorage.StatList.topspeed] + (Information.TSD * tsc);
        }
    }

    public int Acceleration
    {
        get
        {
            return Status[DesignConstStorage.StatList.accelerating];
        }
    }

    public int TotalAC
    {
        get
        {
            return Status[DesignConstStorage.StatList.accelerating] + (Information.ACD * acc);
        }
    }

    public int Cornering
    {
        get
        {
            return Status[DesignConstStorage.StatList.cornering];
        }
    }
    public int TotalCO
    {
        get
        {
            return Status[DesignConstStorage.StatList.cornering] + (Information.COD * coc);
        }
    }

    public int Brake
    {
        get
        {
            return Status[DesignConstStorage.StatList.brake];
        }
    }
    public int TotalBR
    {
        get
        {
            return Status[DesignConstStorage.StatList.brake] + (Information.BRD * brc);
        }
    }

    private string carName;
    public string CarName
    {
        get
        {
            return carName;
        }
    }

    private float minAcceleration = 0;
    public float MinAcceleration
    {
        get
        {
            return minAcceleration;
        }
    }

    private float minDeceleration = 0;
    public float MinDeceleration
    {
        get
        {
            return minDeceleration;
        }
    }

    private float maxHP = 0;
    public float MaxHP
    {
        get
        {
            return maxHP;
        }
    }

    private int tsc;
    public int Tsc
    {
        get
        {
            return tsc;
        }
    }

    private int acc;
    public int Acc
    {
        get
        {
            return acc;
        }
    }

    private int coc;
    public int Coc
    {
        get
        {
            return coc;
        }
    }

    private int brc;
    public int Brc
    {
        get
        {
            return brc;
        }
    }


    //private Car_Information information;

    public Car_Information Information
    {
        get
        {
            return Game_Manager.Instance.CarStat[CarName];
        }
    }

    private Dictionary<DesignConstStorage.StatList, int> Status;

    public void Initialize(TableHandler.Row data)
    {
        carName = data.Get<string>("index");

        price = data.Get<int>("price");

        Status = new Dictionary<DesignConstStorage.StatList, int>();

        for (int i = 0; i <= (int)DesignConstStorage.StatList.accelerating; i++)
        {
            SetStat((DesignConstStorage.StatList)i, data.Get<int>(((DesignConstStorage.StatList)i).ToString()));
        }

        tsc = data.Get<int>("tsc");
        acc = data.Get<int>("acc");
        coc = data.Get<int>("coc");
        brc = data.Get<int>("brc");

        //ModStatus();
    }

    //public void ModStatus()
    //{
    //    information = ref Game_Manager.Instance.CarStat[CarName];

    //    minAcceleration = TotalAC * DesignConstStorage.AccModifier;
    //    minDeceleration = TotalBR * DesignConstStorage.DecModifier;

    //    maxHP = TotalCO;
    //}

    private void SetStat(DesignConstStorage.StatList stat, int i)
    {
        Status.Add(stat, i);
    }
}