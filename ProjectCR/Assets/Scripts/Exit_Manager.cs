using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Manager : MonoBehaviour
{

    private float count = 0;
    public GameObject Exit_Back, Car, Track, Menu, Store;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu.gameObject.activeSelf)
            {
                if (Exit_Back.gameObject.activeSelf)
                {
                    Application.Quit();
                }
                Exit_Back.gameObject.SetActive(true);
            }

            if (Car.gameObject.activeSelf)
            {
                Car.gameObject.SetActive(false);
                Menu.gameObject.SetActive(true);
            }

            if (Track.gameObject.activeSelf)
            {
                Track.gameObject.SetActive(false);
                Menu.gameObject.SetActive(true);
            }

            if (Store.gameObject.activeSelf)
            {
                Store.gameObject.SetActive(false);
                Menu.gameObject.SetActive(true);
            }

        }

        if (Exit_Back.gameObject.activeSelf)
        {
            count += Time.deltaTime;
        }

        if (count >= 2)
        {
            count = 0;
            Exit_Back.gameObject.SetActive(false);
        }
    }
}
