using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip_Manager : MonoBehaviour
{
    [SerializeField]
    private Text Tip_Text;

    private float Tip_Width;
    private bool stop;

    public void Random_Tip()
    {
        Tip_Text.text = DesignConstStorage.Tip;
        Tip_Text.GetComponent<RectTransform>().sizeDelta = new Vector3(Tip_Text.preferredWidth, 100, 0);
        
        Tip_Width = (Tip_Text.GetComponent<RectTransform>().sizeDelta.x / 2);
        //Debug.LogError("길이" + Tip_Width);
        Tip_Text.GetComponent<RectTransform>().anchoredPosition = new Vector2(Tip_Width, -200f);
        stop = false;
    }

    void Update()
    {
        if (Tip_Text.GetComponent<RectTransform>().anchoredPosition.x > -Tip_Width && stop == false)
        {
            Tip_Text.transform.Translate(-0.1f, 0, 0);
        }
        else if (Tip_Text.GetComponent<RectTransform>().anchoredPosition.x <= -Tip_Width && stop == false)
        {
            stop = true;
            Invoke("Random_Tip", 2f);
        }
    }
}
