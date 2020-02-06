using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_Direction : MonoBehaviour
{   
    private Vector3 target, dir;
    private float time = 0, penalty = 0;
    public GameObject Car_Move, Car_Cam;
    public bool Bonus;
    private string start_time;
    [SerializeField]
    private SpriteRenderer car_component = null;

    [SerializeField]
    private Slider HP_Slider;

    [SerializeField]
    private Text Tip_Text;

    [SerializeField]
    private Image fill;

    private float Red = 70;
    private float Green = 200;
    [SerializeField]
    private Sprite start_Image;
    [SerializeField]
    private Sprite brake_Image;
    [SerializeField]
    private Image brake_Btn_Image;
    [SerializeField]
    private GameObject Start_Btn;
    [SerializeField]
    private GameObject Brake_Image;
    [SerializeField]
    private Text Balance_Text;
    private Car_Manager car_manager
    {
        get
        {
            if (Game_Manager.Instance.car_manager != null)
            {
                return Game_Manager.Instance.car_manager;
            }
            else
                return null;
        }
    }
    public float angle;


    public float Penalty
    {
        get
        {
            return penalty;
        }
    }
    public enum Drive_State
    {
        Wait,
        Acceleration,
        Deceleration,
        Out_of_control
    };

    public bool isLastSection
    {
        get
        {
            if (trackPostions != null && sectionIndex >= trackPostions.Count - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum Moving_State
    {
        Parking,
        OnStraight,
        OnCurve,
        OutOfControl
    }

    public enum FinishType
    {
        Finish,
        OutOfControl,
        PassThrough,
        FalseStart
    }

    private Moving_State m_State;
    public Moving_State M_State
    {
        get
        {
            return m_State;
        }
        set
        {
            m_State = value;
        }
    }
    
    //private int topspeed, acceleration, cornering, brake;
    private Drive_State d_State;
    public Drive_State _Drive_State
    {
        set
        {
            d_State = value;
        }
        get
        {
            return d_State;
        }
    }

    private bool isGamePlaying = false;

    private PlayingCarModifier current_Car;

    private List<Vector3[]> trackPostions;

    [HideInInspector]
    public Vector3[] currentSection
    {
        get
        {
            return trackPostions[sectionIndex];
        }
    }

    public float CurrentSpeed = 0;

    private int sectionIndex = 0;
    private int destinationIndex = 0;

    //private GameObject /*gO*/;

    private Vector3 currentDestination
    {
        get
        {
            return currentSection[destinationIndex];
        }
    }

    private float detectionRange
    {
        get
        {
            return DesignConstStorage.DetectionRange;
        }
    }

    private float hpPercent
    {
        get
        {
            return currentHP / current_Car.MaxHP;
        }
    }

    private float currentHP;

    private float curveAngle = 0;
    private float curveDamage = 0;
    public float scoreModifire = 0;

    private bool isBooster;
    public bool IsBooster
    {
        get
        {
            return isBooster;
        }
        set
        {
            isBooster = value;
        }
    }

    private const string path = "Image/CarOutFit/{0}";
    
    private readonly Vector3 baseObjectSize = new Vector3(10, 10, 1);
    
    public void CarInit(PlayingCarModifier cStat)
    {
        current_Car = cStat;

        isBooster = false;

        float sizeFactor = 1;

        switch (current_Car.ModedCarStat.Information.Size)
        {
            case Car_Information.CarSize.Big:
                sizeFactor = 1.1f;
                break;
            case Car_Information.CarSize.Normal:
                sizeFactor = 1.0f;
                break;
            case Car_Information.CarSize.Compact:
                sizeFactor = 0.9f;
                break;
            case Car_Information.CarSize.Small:
                sizeFactor = 0.8f;
                break;
        }
        
        this.gameObject.transform.localScale = baseObjectSize * sizeFactor;


        trackPostions = Game_Manager.Instance.track_manager.Section_vecs;
        
        CurrentSpeed = 0;
        curveAngle = 0;
        curveDamage = 0;

        car_component.sprite = Resources.Load<Sprite>(string.Format(path, current_Car.ModedCarStat.CarName));

        isGamePlaying = true;

        spots = new List<GameObject>();

        currentHP = current_Car.MaxHP;

        sectionIndex = 0;
        this.gameObject.transform.position = currentSection[0];

        destinationIndex = 0;

        d_State = Drive_State.Wait;
        m_State = Moving_State.Parking;
        //Load_Status();
        //Debug.LogError(string.Format("{0} {1} {2} {3}", current_Car.TopSpeed, current_Car.Acceleration, current_Car.Brake, current_Car.Cornering));

        SetDestination();
        Car_Rotation();

        Red = 70;
        Green = 200;
        brake_Btn_Image.sprite = start_Image;
        Balance_Text.text = MyLocalization.Exchange("balance");
        Start_Btn.SetActive(true);
        Brake_Image.SetActive(false);
    }

    public void SetState()
    {
        car_component.transform.Reset();

        d_State = Drive_State.Wait;
        m_State = Moving_State.Parking;
    }
    
    void FixedUpdate()
    {
        if (!isGamePlaying)
        {
            if (m_State == Moving_State.OutOfControl)
            {
                if (isTurnRight)
                {
                    this.transform.position += Vector3.right;
                    car_component.transform.Rotate(Vector3.back * Time.deltaTime * 400);
                }
                else
                {
                    this.transform.position += Vector3.left;
                    car_component.transform.Rotate(Vector3.forward * Time.deltaTime * 400);
                }
            }
            
            return;
        }

        HP_Slider.maxValue = current_Car.MaxHP;
        HP_Slider.value = currentHP;

        float hp = HP_Slider.maxValue / 100;

        if ( ((current_Car.MaxHP - HP_Slider.value) * hp) <= 50f)    // hp가 50프로 이상 레드를 조정
        {
            Red = 70 + (((current_Car.MaxHP - HP_Slider.value) * hp) * 2.6f);
        }
        else// hp가 50프로 이하 그린을 조정
        {
            Green = 200 - ((((current_Car.MaxHP / 2) - HP_Slider.value) * hp) * 2.6f);
        }
        
        fill.color = new Color(Red / 255f, Green / 255f, 70 / 255f);

        MoveCar();

        ChangeColor();

        //if( time <= 30)
        //{
        //    Color();
        //}
        //else if (time > 30 && time < 60)
        //{
        //    Color2();
        //}else if( time >= 60)
        //{
        //    time = 0;
        //}
    }

    private bool isTurnRight = false;

    private void ChangeColor()
    {
        time += 0.1f;
        Color targetColor = new Color(Color.red.r, 1 * hpPercent, 1 * hpPercent);

        car_component.color = Color.Lerp(Color.white, targetColor, (time % 1));
    }

    private List<GameObject> spots = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == Game_Manager.Instance.track_manager.StartLine.name)
        {
            
        }
        else if (collision.gameObject.name == Game_Manager.Instance.track_manager.FinishLine.name)
        {
            Game_Manager.Instance.record_manager.Record = false;
        }
        else
        {
            // Parking spot
        }

        if (!spots.Contains(collision.gameObject))
        {
            spots.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        spots.Remove(collision.gameObject);
    }
    
    private void MoveCar()
    {
        switch (d_State)
        {
            case Drive_State.Wait:
                return;
            case Drive_State.Acceleration:
            case Drive_State.Deceleration:
                break;
            case Drive_State.Out_of_control:
                //여기서는 그냥 뺑뺑이로 밖으로 튀어나가는 연출 보여줄거고
                //카메라는 고정시킨다.
                //그리고 차가 카메라 밖으로 나가면 레이스 끝내는걸로
                if (!isGamePlaying)
                {
                    return;
                }
                m_State = Moving_State.OutOfControl;
                RaceEnd(FinishType.OutOfControl);
                break;
            default:
                Debug.LogError("Wrong State");
                break;
        }
        CalcCurrentSpeed();
        
        switch (m_State)
        {
            case Moving_State.OnCurve:

                if(_Drive_State == Drive_State.Acceleration)
                {
                    currentHP -= curveDamage;
                }
                else if(_Drive_State == Drive_State.Deceleration)
                {
                    currentHP += current_Car.Brake * 0.01f;
                    if (currentHP >= current_Car.MaxHP)
                    {
                        currentHP = current_Car.MaxHP;
                        time = 0;
                    }
                }
                
                if (currentHP <= 0)
                {
                    _Drive_State = Drive_State.Out_of_control;
                    Debug.LogError("Die");
                }
                break;
            case Moving_State.OnStraight:

                //코너링 능력치 대비 회복량 조정해야한다
                //현재 틱당 1%
                DesignConstStorage.DoItList();
                currentHP += current_Car.Cornering * 0.01f;
                if (currentHP >= current_Car.MaxHP)
                {
                    currentHP = current_Car.MaxHP;
                    time = 0;
                }
                break;
        }

        if (M_State != Moving_State.OutOfControl)
        {
            int _checkfactor = (int)(current_Car.Cornering / CurrentSpeed) + 1;
            _checkfactor = 3;
            float divideSpeed = CurrentSpeed / (_checkfactor + 1);

            //Debug.LogError(string.Format("f{0} c{1} s{2} ds{3}", _checkfactor, current_Car.Cornering, CurrentSpeed, divideSpeed));
            
            //이부분 대대적으로 바꿔야 할수도 있다. 191009 기준
            //왜냐하면 지나간 정보로 판단하고 있어서 정보 전달이 늦을수도 있거든
            //이전 정보가 아니라 미래정보 계산하는 방식으로 변경해야 할수도 있다.
            //아닌가? 중간중간 처리하면 안해도 될거 같기도 하다.
            Quaternion preQuat = gameObject.transform.rotation;

            for (int i = 0; i < _checkfactor; i++)
            {
                SetDestination();
                Car_Rotation();
                MoveTo(divideSpeed);
            }

            if (isLastSection)
            {
                //마지막 섹션이면 속도 0 되었을때 결과 팝업
                if (CurrentSpeed <= 0.0f && m_State != Moving_State.Parking)
                {
                    RaceEnd(FinishType.Finish);
                }
            }
            else
            {
                //마지막 섹션이 아니니까 브레이크 띄면 다시 주행
            }

            curveAngle = Quaternion.Angle(preQuat, gameObject.transform.rotation);

            isTurnRight = (preQuat.eulerAngles - gameObject.transform.eulerAngles).z >= 0;
        }
        
        CheckMovingState();
    }

    private void CheckMovingState()
    {
        //이 수치의 의미는 회전각 0.3% 미만은 코너가 아니라고 판단한다는 소리다.
        //조만간 현속 - 코너링 스펙을 비교해서 값 변화하도록 해야한다.
        //아무리 급해도 각도가 3 이상 올라가지는 않는듯.
        //DesignConstStorage.DoItList();
        float dVelue = (current_Car.Cornering * 0.3f) / Mathf.Max(1.0f, CurrentSpeed);
        curveDamage = 0;
        
        switch (m_State)
        {
            case Moving_State.Parking:
            case Moving_State.OnStraight:
            case Moving_State.OnCurve:
                if (curveAngle < dVelue)
                {
                    M_State = Moving_State.OnStraight;
                }
                else
                {
                    M_State = Moving_State.OnCurve;

                    curveDamage = Mathf.Abs(dVelue - curveAngle) * DesignConstStorage.cDamageFactor;
                }
                break;
            case Moving_State.OutOfControl:
                return;
            default:
                Debug.LogError("Wrong Value.");
                break;
        }
    }

    public void RaceEnd(FinishType finishState)
    {
        if (!isGamePlaying)
        {
            return;
        }

        switch (finishState)
        {
            case FinishType.Finish:
                Spots_Count();

                float personalTrackRecord = DesignConstStorage.PersonalTrackRecord[Game_Manager.Instance.track_manager.Current_Track_Name];
                if (personalTrackRecord > Game_Manager.Instance.record_manager.Record_Time)
                {
                    Game_Manager.Instance.SetTimeRecord(Game_Manager.Instance.track_manager.Current_Track_Name, Game_Manager.Instance.record_manager.Record_Time);
                    Debug.LogWarning(Game_Manager.Instance.track_manager.Current_Track_Name);
                }
                break;
            case FinishType.FalseStart:
                Game_Manager.Instance.ui_manager.Fail_Racing_Reason_Text.text = MyLocalization.Exchange("falsestart");
                Tip_Text.text = MyLocalization.Exchange("tip_1");
                break;
            case FinishType.OutOfControl:
                Game_Manager.Instance.ui_manager.Fail_Racing_Reason_Text.text = MyLocalization.Exchange("speeding");
                Tip_Text.text = MyLocalization.Exchange("tip_2");
                break;
            case FinishType.PassThrough:
                Game_Manager.Instance.ui_manager.Fail_Racing_Reason_Text.text = MyLocalization.Exchange("brakefailure");
                Tip_Text.text = MyLocalization.Exchange("tip_3");
                break;
        }
        
        isGamePlaying = false;

        //시간 계산.
        //시간 기반으로 점수 구해서 할까 아니면 그냥 시간+@를 할까?
        //게임 보상도 넣어야..

        //팝업 결과창
        //확인 하면 UI로

        Game_Manager.Instance.EndOfRace(finishState == FinishType.Finish);
    }


    private void Spots_Count()
    {
        if (spots.Exists(gameObject => gameObject.name == "master"))
        {
            Bonus = true;
            switch (spots.Count)
            {
                case 1: // 마스터만 있는경우
                    penalty = 1f;
                    Game_Manager.Instance.record_manager.Record_Time -= penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[0];
                    break;
                case 2: // 마스터 + 1개 
                    penalty = 0.6f;
                    Game_Manager.Instance.record_manager.Record_Time -= penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[1];
                    break;
                case 3: // 마스터 + 2개
                    penalty = 0.2f;
                    Game_Manager.Instance.record_manager.Record_Time -= penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[2];
                    break;
                default:
                    Debug.LogWarning(spots.Count);
                    penalty = 0f;
                    Game_Manager.Instance.record_manager.Record_Time -= penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[5];
                    break;
            }
        }
        else // 마스터 미포함
        {
            Bonus = false;
            switch (spots.Count)
            {
                case 0: // 끝에 0개 걸침
                    penalty = 1f;
                    Game_Manager.Instance.record_manager.Record_Time += penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[5];
                    break;
                case 1: // 끝에 1개 걸침
                    penalty = 0.1f;
                    Game_Manager.Instance.record_manager.Record_Time += penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[4];
                    break;
                case 2: // 끝에 2개 걸침
                    penalty = 0f;
                    Game_Manager.Instance.record_manager.Record_Time += penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[3];
                    break;
                default:
                    Debug.LogWarning("말도안됌");
                    penalty = -1f;
                    Game_Manager.Instance.record_manager.Record_Time += penalty;
                    Game_Manager.Instance.ui_manager.money = DesignConstStorage.gameMoneyReward[5];
                    break;
            }
        }
    }
    private void CalcCurrentSpeed()
    {
        Game_Manager.Instance.sound_manager.audioSource.volume = 0.1f + (( (90f / current_Car.TopSpeed) * CurrentSpeed) * 0.01f);     // 차량 사운드 조절

        switch (d_State)
        {
            case Drive_State.Acceleration:
                float currentAcc = ((current_Car.TopSpeed - CurrentSpeed) / Mathf.Max(1, CurrentSpeed)) + Mathf.Max(1, (current_Car.Acceleration - (((current_Car.TopSpeed * 0.5f) + current_Car.Acceleration) * 0.5f)) * DesignConstStorage.AccelerationFactor);//(current_Car.Acceleration * (1 - (CurrentSpeed / current_Car.TopSpeed))) + current_Car.MinAcceleration;

                //Debug.LogError(string.Format("{0}   {1}  {2}", ((current_Car.TopSpeed - CurrentSpeed) / Mathf.Max(1, CurrentSpeed)), Mathf.Max(1, (current_Car.Acceleration - (((current_Car.TopSpeed * 0.5f) + current_Car.Acceleration) * 0.5f)) * 0.5f), currentAcc));

                if (IsBooster)
                {
                    currentAcc *= 1.5f;
                }

                if (m_State == Moving_State.OnCurve)
                {
                    //Debug.LogError((1 - (curveAngle * 0.01f)));
                    currentAcc = currentAcc * (1 - (curveAngle * 0.01f));
                }

                CurrentSpeed = Mathf.Min(current_Car.TopSpeed, (CurrentSpeed + (currentAcc * Time.deltaTime)));

                //Debug.LogError(string.Format("{0}    {1}", currentAcc, CurrentSpeed));

                break;
            case Drive_State.Deceleration:
                float contrast = current_Car.MinDeceleration + (current_Car.Brake * (DesignConstStorage.DecelerationFactor * Mathf.Acos(Mathf.Min(((CurrentSpeed / current_Car.Brake) - 1), 1))));

                //Debug.LogError(string.Format("{0}    {1}   {2}", (CurrentSpeed / current_Car.Brake), ((CurrentSpeed / current_Car.Brake) - 1), Mathf.Acos(Mathf.Min(((CurrentSpeed / current_Car.Brake) - 1), 1))));
                
                //Debug.LogError(contrast);

                CurrentSpeed = Mathf.Max(0, (CurrentSpeed - (contrast * Time.deltaTime)));

                break;
            default:
                break;
        }

        //Debug.LogError(CurrentSpeed);

        //무슨생각으로 여기서 겜 끝내냐
        //if (m_State == Moving_State.GameOver)
        //{
        //    RaceEnd();
        //}
    }

    private void MoveTo(float speed)
    {
        this.gameObject.transform.position += dir * DesignConstStorage.MovingUnit * speed;
    }

    private void SetDestination()
    {
        while (Vector3.SqrMagnitude(currentDestination - this.gameObject.transform.position) < (detectionRange * Mathf.Max(1, CurrentSpeed * 0.01f)))
        {
            //new GameObject().transform.position = currentDestination;
            destinationIndex++;
            if (destinationIndex >= DesignConstStorage.SectionSize)
            {
                if (isLastSection)
                {
                    RaceEnd(FinishType.PassThrough);
                }
                else
                {
                    sectionIndex++;
                    destinationIndex = 0;
                }

            }
        }
    }

    //private void Color()
    //{
    //    if (current_Car != null)
    //    {
    //        car_component.color = new Color(255f / 255f, currentHP * (255f / current_Car.MaxHP) / 255f, currentHP * (255f / current_Car.MaxHP) / 255f);
    //    }
    //}

    //private void Color2()
    //{
    //    car_component.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    //}

    public void Car_Rotation()
    {
        dir = (currentDestination - this.transform.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, (angle - 90));
    }

    public void Change_Brake_Button_Image()
    {
        brake_Btn_Image.sprite = brake_Image;
        Start_Btn.SetActive(false);
        Brake_Image.SetActive(true);
    }
}