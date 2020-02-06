using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Car_Manager : MonoBehaviour
{

    //public Game_Manager game_Manager;
    public Car_Direction car_direstion;
    private const string path = "Image/{0}";
    private Sprite[] threeTwoOne;
    private Sprite zero;

    [SerializeField]
    private Image SecDisplayer = null;

    private bool Up_Brake_Btn = false;


    [SerializeField]
    private GameObject Brake_Image;

    private PlayingCarModifier currentCar
    {
        get
        {
            if (Game_Manager.Instance.PCM != null)
            {
                return Game_Manager.Instance.PCM;
            }
            else
            {
                return null;
            }
            
        }
    }

    public void Initialize()
    {
        car_direstion.gameObject.SetActive(false);
        SecDisplayer.gameObject.SetActive(false);
        threeTwoOne = new Sprite[3];
        threeTwoOne[0] = Resources.Load<Sprite>(string.Format(path, "Three"));
        threeTwoOne[1] = Resources.Load<Sprite>(string.Format(path, "Two"));
        threeTwoOne[2] = Resources.Load<Sprite>(string.Format(path, "One"));

        zero = Resources.Load<Sprite>(string.Format(path, "Zero"));
    }

    public void CarInit()
    {
        isOnCountDown = false;
        Up_Brake_Btn = false;

        car_direstion.CarInit(currentCar);

        Brake_Image.SetActive(false);
        car_direstion.gameObject.SetActive(true);
    }

    public void BreakDown()
    {
        if (car_direstion.M_State != Car_Direction.Moving_State.Parking)
        {
            car_direstion._Drive_State = Car_Direction.Drive_State.Deceleration;
            Brake_Image.SetActive(true);
        }
        else if(!Up_Brake_Btn)
        {
            StartCoroutine(CountDown());
        }
    }

    public void BreakUp()
    {

        if (isOnCountDown && !Game_Manager.Instance.setting_manager.Auto_Start)
        {
            car_direstion.RaceEnd(Car_Direction.FinishType.FalseStart);
        }

        if(!isOnCountDown)
        {
            car_direstion._Drive_State = Car_Direction.Drive_State.Acceleration;
            Brake_Image.SetActive(false);
        }

        Up_Brake_Btn = true; // 카운트 중간에 버튼을 땠는지 여부

        if (Game_Manager.Instance.record_manager.Record)
        {
            Game_Manager.Instance.sound_manager.Racing_Start();     //게임중일때만 사운드 켜짐
        }
    }



    private bool isOnCountDown = false;
    public bool IsOnCountDown
    {
        get
        {
            return isOnCountDown;
        }
        set
        {
            isOnCountDown = value;
        }
    }


    private IEnumerator auto()
    {
        while (Game_Manager.Instance.record_manager.Record_Time <= 0.3f)
        {
            yield return new WaitForSeconds(0.01f);
        }

        Game_Manager.Instance.record_manager.Start_Record_Bool = true; // 스타트 인터벌 체크
        Game_Manager.Instance.record_manager.Check_Start_Interval();
        Game_Manager.Instance.sound_manager.Racing_Start(); // 사운드

        car_direstion._Drive_State = Car_Direction.Drive_State.Acceleration;
    }


    private IEnumerator CountDown()
    {
        isOnCountDown = true;
        SecDisplayer.color = new Color(1, 1, 1, 1);
        SecDisplayer.gameObject.SetActive(true);
        float timePassed = 0;

        for (int i = 0; i < threeTwoOne.Length; i++)
        {
            SecDisplayer.sprite = threeTwoOne[i];
            SecDisplayer.transform.Reset();
            SecDisplayer.transform.localScale = new Vector3(3, 3, 3);

            timePassed = 0;

            while (timePassed < 1)
            {
                if (isOnCountDown)
                {
                }
                else
                {
                    break;
                }

                yield return new WaitForFixedUpdate();

                timePassed += Time.deltaTime;

                SecDisplayer.transform.localPosition = new Vector3(SecDisplayer.transform.localPosition.x + (Time.deltaTime * 1000), SecDisplayer.transform.localPosition.y - (Time.deltaTime * 25), 0);

                float tempScale = SecDisplayer.transform.localScale.x - (Time.deltaTime * 1.5f);

                SecDisplayer.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            }
        }

        if (isOnCountDown)
        {
            isOnCountDown = false;

            Game_Manager.Instance.record_manager.Racing_Record_Start();

            if (Game_Manager.Instance.setting_manager.Auto_Start && Up_Brake_Btn) // 오토가 켜져있고 버튼을 중간에 땠으면 오토출발 실행 / 오토가 켜져있어도 카운트 가 끝나기전까지 브레이크를 떼지 않으면 실행 안함
            {
                StartCoroutine(auto());
            }

            SecDisplayer.sprite = zero;
            SecDisplayer.transform.Reset();
            SecDisplayer.transform.localScale = new Vector3(3, 3, 3);

            timePassed = 0;

            while (timePassed < 1)
            {
                yield return new WaitForFixedUpdate();

                timePassed += Time.deltaTime;

                SecDisplayer.color = new Color(SecDisplayer.color.r, SecDisplayer.color.g, SecDisplayer.color.b, SecDisplayer.color.a - Time.deltaTime);
                //float tempScale = SecDisplayer.transform.localScale.x - (Time.deltaTime * 1.5f);

                //SecDisplayer.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            }
        }
        else
        {
        }

        SecDisplayer.gameObject.SetActive(false);
    }
}