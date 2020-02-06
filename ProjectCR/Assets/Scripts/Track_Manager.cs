using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Track_Manager : MonoBehaviour
{
    public Path_Manager[] MapList = null;

    [SerializeField]
    private Transform mapPostion = null;
    public MeshRenderer material;
    private string Map_Name;
    private Game_Manager game_manager = Game_Manager.Instance;
    private Path_Manager current_track;
    //private int Section_size = DesignConstStorage.SectionSize;
    //private float t;
    public List<Vector3[]> Section_vecs;

    public SpriteRenderer CheckerLine = null;

    public string Current_Track_Name
    {
        get
        {
            return Map_Name;
        }
    }
    [SerializeField]
    private FinishSpotDetector ParkingSpot = null;

    private SpriteRenderer startLine = null;
    public SpriteRenderer StartLine
    {
        get
        {
            return startLine;
        }
    }

    private SpriteRenderer finishLine = null;
    public SpriteRenderer FinishLine
    {
        get
        {
            return finishLine;
        }
    }
    
    public int TPos = 0;
    
    private Dictionary<string, Path_Manager> trackList;

    public void Initialize()
    {
        trackList = new Dictionary<string, Path_Manager>();
        TrackListInit();

        if (startLine == null)
        {
            startLine = Instantiate(CheckerLine).GetComponent<SpriteRenderer>();
            startLine.gameObject.name = DesignConstStorage.sLine;
            finishLine = Instantiate(CheckerLine).GetComponent<SpriteRenderer>();
            finishLine.gameObject.name = DesignConstStorage.fLine;
            ParkingSpot = Instantiate(ParkingSpot).GetComponent<FinishSpotDetector>();
        }
        

        ParkingSpot.Initialize();
        Game_Manager.Instance.record_manager.Record = false;
        startLine.gameObject.transform.SetParent(this.transform);
        finishLine.gameObject.transform.SetParent(this.transform);
        ParkingSpot.gameObject.transform.SetParent(this.transform);
        startLine.gameObject.SetActive(false);
        finishLine.gameObject.SetActive(false);
        ParkingSpot.gameObject.SetActive(false);

        TrackSelect();

        Save_Track();
    }

    public void Save_Track()
    {
        for (int i = 0; i < DesignConstStorage.TrackDataTable.Rows.Count; i++)
        {
            if (DesignConstStorage.TrackDataTable.Rows[i].Get<string>("index") == PlayerPrefs.GetString("select_map", DesignConstStorage.TrackDataTable.Rows[0].Get<string>("index")))
            {
                TrackSelect(DesignConstStorage.TrackDataTable.Rows[i]);
            }
        }
    }
    private void TrackListInit()
    {
        foreach (var map in MapList)
        {
            map.Initialize();
            trackList.Add(map.MapName, map);
        }
    }

    public void TrackSelect(TableHandler.Row trackData = null)
    {
        if (trackData == null)
        {
            trackData = DesignConstStorage.TrackDataTable.Rows[UnityEngine.Random.Range(0, DesignConstStorage.TrackDataTable.Rows.Count)];

            trackData = DesignConstStorage.TrackDataTable.Rows[UnityEngine.Random.Range(0, 1)];
        }

        string tName = trackData.Get<string>("mapname");
        Map_Name = trackData.Get<string>("index");
        int startSection = trackData.Get<int>("startsection");
        int sectionLength = trackData.Get<int>("sectionlength");

        current_track = trackList[tName];
        Set_current_track(startSection, sectionLength);

        //Debug.Log("선택한 트랙 : " + string.Format("Track : {0}  StartSection : {1}  Length : {2}", tName, startSection, sectionLength));
    }
    
    private void Set_current_track(int Start_Node, int section_Length)
    {
        Section_vecs = new List<Vector3[]>();

        //여기에 왜 섹션 길이보다 + 2 받았을까? 마지막 포인트 포함해서 +1이면 되는데..
        for (int i = 0; i <= section_Length; i++)
        {
            //current_track.Add(game_manager.path_manager.Node_List[(Start_Node + i) % game_manager.path_manager.Node_List.Count]);
            Section_vecs.Add(current_track.DividedSection[(Start_Node + i) % current_track.DividedSection.Count]);
            //GameObject gO = new GameObject();
            //gO.transform.position = Section_vecs[i][0];
            //gO.name = i.ToString();
        }
    }

    public Mesh GetMapMesh(string mName)
    {
        return trackList[mName].Creator.TossMesh();
    }

    public Rect GetMapRect(string mName)
    {
        //Rect r = new Rect(trackList[mName].Creator.gameObject.transform.position, trackList[mName].Creator.TossMeshSize());
        return new Rect(trackList[mName].Creator.gameObject.transform.position, trackList[mName].Creator.TossMeshSize());
    }

    public void Car_Cam()
    {

    }

    public void TrackInit()
    {
        startLine.gameObject.SetActive(true);
        finishLine.gameObject.SetActive(true);
        ParkingSpot.gameObject.SetActive(true);
        //var asdasd = Section_vecs.ToArray();0
        mapPostion.GetComponent<MeshFilter>().mesh = GetMapMesh(current_track.MapName);

        SetLineAndFinishPoint();

        ParkingSpot.ReSetflags();
        Game_Manager.Instance.record_manager.Record_Time = 0;
    }

    private void SetLineAndFinishPoint()
    {
        SetcheckerLine(startLine.gameObject, Section_vecs[0]);
        SetcheckerLine(finishLine.gameObject, Section_vecs[Section_vecs.Count - 1]);

        Vector3[] tSection = Section_vecs[Section_vecs.Count - 1];
        TPos = UnityEngine.Random.Range(50, 100);
        ParkingSpot.gameObject.transform.position = tSection[TPos];
        Vector3 dir = (tSection[TPos] - tSection[TPos + 1]).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ParkingSpot.gameObject.transform.rotation = Quaternion.Euler(0, 0, (angle - 90));
    }

    private void SetcheckerLine(GameObject gO, Vector3[] tSection)
    {
        int Setpos = 30;
        gO.transform.position = tSection[Setpos];
        Vector3 dir = (tSection[Setpos] - tSection[Setpos + 1]).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gO.transform.rotation = Quaternion.Euler(0, 0, (angle - 90));
    }
    
    public Vector3[] CalculateEvenlySpacedPoints(float resolution = 1)//float spacing
    {
        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        
        evenlySpacedPoints.Add(game_manager.path_manager.Node_List[0].transform.position);
        Vector3 previousPoint = game_manager.path_manager.Node_List[0].transform.position;
        //float dstSinceLastEvenPoint = 0;
        
        return evenlySpacedPoints.ToArray();
    }
    public int NumSegments
    {
        get 
        {
            return game_manager.path_manager.Node_List.Count;
        }
    }

    public void Track_Reset()
    {
        mapPostion.GetComponent<MeshFilter>().mesh = null;
        startLine.gameObject.SetActive(false);
        finishLine.gameObject.SetActive(false);
        ParkingSpot.gameObject.SetActive(false);
        Game_Manager.Instance.record_manager.Record = false;
    }
}
