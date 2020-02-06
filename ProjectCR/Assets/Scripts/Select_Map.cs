using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Select_Map : MonoBehaviour
{

    public GameObject Map_Btn_1;

    public GameObject Map_Btn_2;

    public GameObject Map_Btn_3;

    public GameObject Record_Text;

    public GameObject Pooling_Parents;

    public GameObject Record_Parents;

    public GameObject UI_Parents;

    public string Current_Track_Name;


    public Text Map_Page_T;

    [SerializeField]
    private Text PersonalTrackRecord_Text;
    [SerializeField]
    private Text PersonalTrackRecord_Text2;
    [SerializeField]
    private Text current_Track_Text;
    [SerializeField]
    private Text current_Track_Text2;
    [SerializeField]
    private Text track_Name_Text0;
    [SerializeField]
    private Text track_Name_Text1;
    [SerializeField]
    private Text track_Name_Text2;

    [SerializeField]
    private List<GameObject> Map_Btn_List = new List<GameObject>();

    [SerializeField]
    private GameObject select_Track_UI;
    [SerializeField]
    private GameObject Menu_UI;
    [SerializeField]
    

    public int map = 0;
    public int select = 0;
    private int road_Record;
    private int Page = 0;
    private int show_Track;
    private int track_count;

    private float scale_x = 2;
    private float scale_y = 2;

    private Path_Manager current_track;
    public Game_Manager game_manager;

    private Stack<GameObject> pooledObject = null;

    private bool coroutine = true;

    public List<GameObject> pool_List = new List<GameObject>();
    public List<string> Roading_Server_List = new List<string>();

    private UI_Manager ui;
    public void Initialized()
    {
        pooledObject = new Stack<GameObject>();
        track_count = DesignConstStorage.TrackDataTable.Rows.Count;
        ui = Game_Manager.Instance.ui_manager;

        if (track_count % 3 == 0)
        {
            Page = track_count / 3;
        }
        else
        {
            Page = (track_count / 3) + 1;
        }
        Map_Page_T.text = ((map / 3) + 1) + " / " + Page;

        for (int i = 1; i < 50; i ++) // 50은 임의의 값
        {
            GameObject record_Text = Instantiate(Record_Text, new Vector2(0, 0), Quaternion.identity);
            //record_Text.SetActive(false);
            record_Text.transform.SetParent(Pooling_Parents.transform, false);
            pooledObject.Push(record_Text);
        }
        select = PlayerPrefs.GetInt("select", 0);
        map = PlayerPrefs.GetInt("map", 0);
        Active_Track_Btn();
        PersonalRecord(select);
        StartCoroutine(get_server_record(PlayerPrefs.GetString("select_map", DesignConstStorage.TrackDataTable.Rows[0].Get<string>("index")), UI_Parents));
        current_Text(map);
        Track_Mesh(map);
        MapScale();
        Current_Track_Name = PlayerPrefs.GetString("select_map", DesignConstStorage.TrackDataTable.Rows[0].Get<string>("index"));
    }


    public IEnumerator get_server_record(string index, GameObject parents)
    {
        if (Roading_Server_List.Contains(index))
        {
            Make_Record_Text(index, parents);
            Debug.LogWarning("asdasd");
            yield break;
        }
        else
        {
            coroutine = true;
        }


        if(Roading_Server_List.Count == 0)
        {
            coroutine = true;
        }

        if(coroutine)
        {
            Roading_Server_List.Add(index);
        }


        road_Record = Game_Manager.Instance.GetTrackRecord(index).Count;
        //Debug.LogError(road_Record + " :서버 기록 부르기 첫 시도");
        if (road_Record != 0)
        {
            //Debug.LogError(road_Record + " :서버 기록 부르기 성공");
            Make_Record_Text(index, parents);
            Roading_Server_List.RemoveAt(Roading_Server_List.IndexOf(index));
            yield break;
        }
        else
        {
            Make_Record_Text(index, parents);
        }

        while (road_Record == 0 && coroutine)
        {
            yield return new WaitForSeconds(0.5f);
            road_Record = Game_Manager.Instance.GetTrackRecord(index).Count;
            if(road_Record > 0)
            {
                if(Current_Track_Name == index)
                {
                    Make_Record_Text(index, parents);
                }
                yield break;
            }
        }
    }

    public void Click_N_Btn()
    {
        if (track_count >= map + 3) // 19는 임시데이터
        {
            map += 3;
            int i;
            if(track_count - map >= 3)
            {
                i = 0;
            }
            else
            {
                i = track_count - map;
            }
            Active_Track_Btn();
            MapScale();
            current_Text(map);
            Track_Mesh(map);
        }
    }

    public void Click_P_Btn()
    {
        if (map != 0)
        {
            map -= 3;
            Active_Track_Btn();

            MapScale();
            current_Text(map);
            Track_Mesh(map);
        }
    }

    public void Make_Record_Text(string trackname, GameObject parents)
    {
        for(int i = 0; i < pool_List.Count; i++)
        {
            pool_List[i].transform.SetParent(Pooling_Parents.transform, false);
            pooledObject.Push(pool_List[i]);
        }

        pool_List.Clear();

        List<float> server_record = Game_Manager.Instance.GetTrackRecord(trackname);

        if (server_record.Count >= 1)
        {
            for (int i = 0; i < server_record.Count; i++)
            {
                GameObject Record_Text = pooledObject.Pop();
                Record_Text.transform.SetParent(parents.transform, false);
                switch(i)
                {
                    case 0:
                        Record_Text.GetComponent<Text>().text = (i + 1) + "st.   " + server_record[i].ToString("N3") + "s";
                        break;
                    case 1:
                        Record_Text.GetComponent<Text>().text = (i + 1) + "nd.   " + server_record[i].ToString("N3") + "s";
                        break;
                    case 2:
                        Record_Text.GetComponent<Text>().text = (i + 1) + "rd.   " + server_record[i].ToString("N3") + "s";
                        break;
                    default:
                        Record_Text.GetComponent<Text>().text = (i + 1) + "th.   " + server_record[i].ToString("N3") + "s";
                        break;
                }
                pool_List.Add(Record_Text);
            }
        }
        else
        {
            GameObject Record_Text = pooledObject.Pop();
            Record_Text.transform.SetParent(parents.transform, false);
            Record_Text.GetComponent<Text>().text = "서버에서 기록을 불러오는 중입니다.";
            pool_List.Add(Record_Text);
            //Debug.LogError(server_record.Count);
        }
    }
    private void startCoroutine(string index)
    {
        StartCoroutine(get_server_record(index , Record_Parents));
        Current_Track_Name = index;
    }

    public void Click_Track()
    {
        for(int i = 0; i < 3; i++)
        {
            if(EventSystem.current.currentSelectedGameObject.name == "Map_Btn" + i)
            {
                select = map + i;
                Game_Manager.Instance.ui_manager.mainmap.localScale = new Vector2(ui.track[i].localScale.x * 1.8f, ui.track[i].localScale.y * 1.8f);
            }
        }

        ui.mainmap.GetComponent<MeshFilter>().mesh = Game_Manager.Instance.track_manager.GetMapMesh(map_name(select));
        ui.mainmap.GetComponent<MeshRenderer>().material = ui.Material(map_name(select)).sharedMaterial;
        PersonalRecord(select);
        startCoroutine(DesignConstStorage.TrackDataTable.Rows[select].Get<string>("index"));
        current_Text(map);
    }

    public void PersonalRecord(int i)
    {
        if(DesignConstStorage.PersonalTrackRecord[DesignConstStorage.TrackDataTable.Rows[i].Get<string>("index")] == 0)
        {
            PersonalTrackRecord_Text.text = "아직 기록이 없습니다.";
        }
        else
        {
            PersonalTrackRecord_Text.text = "BEST.  " + DesignConstStorage.PersonalTrackRecord[DesignConstStorage.TrackDataTable.Rows[i].Get<string>("index")].ToString("N3") + "s";
            PersonalTrackRecord_Text2.text = "BEST.  " + DesignConstStorage.PersonalTrackRecord[DesignConstStorage.TrackDataTable.Rows[i].Get<string>("index")].ToString("N3") + "s";
        }
    }
    private void MapScale()
    {
        if (track_count - map >= 3)
        {
            show_Track = map + 3;
        }
        else
        {
            show_Track = map + (track_count - map);
        }

        for (int i = map; i < show_Track; i++)
        {
            ui.track[i % 3].transform.localScale = ui.track_scale(map_name(i));
        }
    }

    void Update()
    {
    }


    public void Select_Track()
    {
        PlayerPrefs.SetInt("select", select);
        PlayerPrefs.SetInt("map", map);
        PlayerPrefs.SetString("select_map", DesignConstStorage.TrackDataTable.Rows[select].Get<string>("index"));

        PersonalRecord(select);
        current_Text(map);
        game_manager.track_manager.Save_Track();
        ui.Select_map.GetComponent<MeshFilter>().mesh = ui.mainmap.GetComponent<MeshFilter>().mesh;
        ui.Select_map.localScale = ui.mainmap.localScale;
        ui.Select_map.GetComponent<MeshRenderer>().material = ui.mainmap.GetComponent<MeshRenderer>().material;
        game_manager.track_manager.material.material = ui.mainmap.GetComponent<MeshRenderer>().material;

        PlayerPrefs.SetFloat("main_map_x", ui.mainmap.localScale.x);
        PlayerPrefs.SetFloat("main_map_y", ui.mainmap.localScale.y);
        PlayerPrefs.SetFloat("select_map_x", ui.track[select % 3].localScale.x * 1.7f);
        PlayerPrefs.SetFloat("select_map_y", ui.track[select % 3].localScale.y * 1.7f);

        StartCoroutine(get_server_record(PlayerPrefs.GetString("select_map"), UI_Parents));
        
    }

    private void Track_Mesh(int i)
    {

        for (int j = 0; j < 3; j++)
        {
            if(Map_Btn_List[j].activeSelf)
            {
                ui.track[j].GetComponent<MeshFilter>().mesh = Game_Manager.Instance.track_manager.GetMapMesh(map_name(i + j));
                ui.track[j].GetComponent<MeshRenderer>().material = ui.Material(map_name(i + j)).sharedMaterial;
            }
        }
        if(Map_Btn_1.gameObject.activeSelf)
            track_Name_Text0.text = map_name(i);

        if (Map_Btn_2.gameObject.activeSelf)
            track_Name_Text1.text = map_name(i + 1); 

        if (Map_Btn_3.gameObject.activeSelf)
            track_Name_Text2.text = map_name(i + 2);

    }

    private void Active_Track_Btn()
    {
        int i;
        if (track_count - map >= 3)
        {
            i = 0;
        }
        else
        {
            i = track_count - map;
        }
        switch (i)
        {
            case 0:
                Map_Btn_1.gameObject.SetActive(true);
                Map_Btn_2.gameObject.SetActive(true);
                Map_Btn_3.gameObject.SetActive(true);
                break;
            case 1:
                Map_Btn_1.gameObject.SetActive(true);
                Map_Btn_2.gameObject.SetActive(false);
                Map_Btn_3.gameObject.SetActive(false);
                break;
            case 2:
                Map_Btn_1.gameObject.SetActive(true);
                Map_Btn_2.gameObject.SetActive(true);
                Map_Btn_3.gameObject.SetActive(false);
                break;
        }
    }
    private void current_Text(int i)
    {
        current_Track_Text.text = MyLocalization.Exchange("currenttracktext") + " : " + map_name(select);
        current_Track_Text2.text = MyLocalization.Exchange("currenttracktext") + " : " + map_name(PlayerPrefs.GetInt("select", 0));
        Map_Page_T.text = ((i / 3) + 1) + " / " + Page;
    }
    public void Click_My_Track()
    {
        map = PlayerPrefs.GetInt("map", 0);
        select = PlayerPrefs.GetInt("select", 0);
        Menu_UI.gameObject.SetActive(false);
        select_Track_UI.gameObject.SetActive(true);

        Active_Track_Btn();
        PersonalRecord(select);
        Track_Mesh(map);
        MapScale();
        current_Text(map);
        startCoroutine(PlayerPrefs.GetString("select_map"));
        ui.mainmap.GetComponent<MeshFilter>().mesh = Game_Manager.Instance.track_manager.GetMapMesh(map_name(select));
        ui.mainmap.GetComponent<MeshRenderer>().material = ui.Material(map_name(select)).sharedMaterial;
        ui.mainmap.localScale = new Vector2(ui.track[select % 3].localScale.x * 1.8f, ui.track[select % 3].localScale.y * 1.8f);
    }

    public void Cancel_Btn()
    {
        Menu_UI.gameObject.SetActive(true);
        select_Track_UI.gameObject.SetActive(false);
        Current_Track_Name = PlayerPrefs.GetString("select_map", DesignConstStorage.TrackDataTable.Rows[0].Get<string>("index"));
        StartCoroutine(get_server_record(PlayerPrefs.GetString("select_map"), UI_Parents));
    }


    private string map_name(int i)
    {
        return DesignConstStorage.TrackDataTable.Rows[i].Get<string>("mapname");
    }
}