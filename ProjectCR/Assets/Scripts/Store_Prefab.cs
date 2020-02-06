using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store_Prefab : MonoBehaviour
{

    public void Click_Category()
    {
        Game_Manager.Instance.store_manager.Click_Category();
        Game_Manager.Instance.sound_manager.Button_Sound();
    }

    public void Click_Item()
    {
        Game_Manager.Instance.store_manager.Click_Item();
        Game_Manager.Instance.sound_manager.Button_Sound();
    }
}
