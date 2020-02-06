using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Money_Manager : MonoBehaviour
{
    public GameObject Money_BackGround;
    public Text GameMoney_Text, Cash_Text;
    public GameObject Ingame, GameMoney, Cash;

    public void Display_Wealth(string gameMoney, string cash)
    {
        GameMoney_Text.text = gameMoney;
        Cash_Text.text = cash;
    }

    public void Money_Active()
    {
        if (Ingame.gameObject.activeSelf)
        {
            Money_BackGround.SetActive(false);
        }
        else
        {
            Money_BackGround.SetActive(true);
        }

        //if(Ingame.gameObject.activeSelf)
        //{
        //    GameMoney.gameObject.SetActive(false);
        //    Cash.gameObject.SetActive(false);
        //}
        //else
        //{
        //    GameMoney.gameObject.SetActive(true);
        //    Cash.gameObject.SetActive(true);
        //}
    }
}


