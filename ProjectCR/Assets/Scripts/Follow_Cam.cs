using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Follow_Cam : MonoBehaviour
{
    public Camera Main_Cam;
    public GameObject Car_Cam; // 추적할 오브젝트의 Transform
    public Transform Car;
    public GameObject Ingame;
    public float dist = 20.0f, height = 10.0f;
    public float smoothRotate = 5f; // 부드러운 회전
    
    private Transform Cam_Tr; // 카메라 자신의 Transform

    private Vector3 targetPos;
    
    void Start()
    {
        Cam_Tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (Ingame.gameObject.activeSelf)
        {
            //dis = game_manager.car_direction.dis;
            //cam_Target_Position = game_manager.track_manager.Section_vecs[count];
            //if (dis >= DesignConstStorage.SectionSize + 20)
            //{
            //    count += 1;
            //}
            //Car_Cam.transform.position = Vector3.Lerp(Car.transform.position, cam_Target_Position[dis], 50);

            if (Game_Manager.Instance.car_manager.car_direstion._Drive_State == Car_Direction.Drive_State.Out_of_control)
            {
                return;
            }

            if (Game_Manager.Instance.backendManager.IsInitialized && Game_Manager.Instance.car_manager.car_direstion.isLastSection)
            {
                targetPos = Vector3.Lerp(transform.position, Game_Manager.Instance.car_manager.car_direstion.currentSection[Game_Manager.Instance.track_manager.TPos], smoothRotate * Time.deltaTime);

                //Debug.LogError(string.Format("{0} {1} {2}  {3}",finishPos, transform.position, targetPos,Time.deltaTime));
            }
            else
            {
                targetPos = Car_Cam.transform.position;
            }

            //float currYAngle = Mathf.LerpAngle(Cam_Tr.eulerAngles.y, Car.eulerAngles.y, smoothRotate * Time.deltaTime);

            //if (currYAngle != 0)
            //{
            //    Debug.LogWarning("");
            //}

            Quaternion rot = Quaternion.Euler(0, 0, 0);

            Cam_Tr.position = targetPos - (rot * Vector3.forward * dist) + (Vector3.up * height);

            Cam_Tr.LookAt(targetPos);

            Main_Cam.orthographicSize = (Game_Manager.Instance.car_direction.CurrentSpeed * 0.3f)+ 50;

            //Debug.LogError(Car_Direction.Drive_State.Deceleration);
            //Car_Cam.transform.localPosition = new Vector2(Car.localPosition.x * 0.02f, 1 + Game_Manager.Instance.car_direction.CurrentSpeed * 0.03f);
            //Car_Cam.transform.localPosition =  자동차의 미래 섹션 백터 값, 
            //섹션사이즈를 넘어가면 다음 섹션값 받아오기.
        }
    }
}
