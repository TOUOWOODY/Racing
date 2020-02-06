using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCarModifier : MonoBehaviour
{
    private Car_Status currentSelected;
    public Car_Status ModedCarStat
    {
        get
        {
            return currentSelected;
        }
    }

    private int topSpeed;
    public int TopSpeed
    {
        get
        {
            return topSpeed;
        }
    }
    private int acceleration;
    public int Acceleration
    {
        get
        {
            return acceleration;
        }
    }
    private int cornering;
    public int Cornering
    {
        get
        {
            return cornering;
        }
    }
    private int brake;
    public int Brake
    {
        get
        {
            return brake;
        }
    }

    private int maxHP = 0;
    public int MaxHP
    {
        get
        {
            return maxHP;
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

    public void Initialize(Car_Status car)
    {
        currentSelected = car;

        topSpeed = SetStat(car.TotalTS);
        acceleration = SetStat(car.TotalAC);
        cornering = SetStat(car.TotalCO);
        brake = SetStat(car.TotalBR);

        minAcceleration = (Acceleration + TopSpeed) * DesignConstStorage.AccModifier;
        minDeceleration = Brake * DesignConstStorage.DecModifier;

        maxHP = Cornering;
    }

    private int SetStat(int value)
    {

        return value;
    }

    public void Initialize()
    {
        Initialize(currentSelected);
    }
}