using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(RoadCreator))]
[ExecuteInEditMode]
public class Path_Manager : MonoBehaviour
{
    public string MapName = null;
    //public List<GameObject> Origin_Pos = null;
    public List<Master_Node> Node_List = null;

    [SerializeField]
    private RoadCreator creator = null;
    public RoadCreator Creator
    {
        get
        {
            return creator;
        }
    }

    private List<Vector3[]> dividedSection = null;
    public List<Vector3[]> DividedSection
    {
        get
        {
            return dividedSection;
        }
    }
    
    [SerializeField]
    private Sprite img;
    //void Start()
    //{
    //    Initialize();
    //}

    public void Initialize()
    {
        MapName = this.gameObject.name;
        Create_Node_List();
        Center();
        //if(Origin_Pos != null && Origin_Pos.Count > 1 && Node_List == null)
        //{
        //    Create_Node_List();
        //}

        //asdasd = DividedSection.Count;
    }

    private void Create_Node_List()
    {
        //Node_List = new List<Master_Node>();

        //foreach (var pos in Origin_Pos)
        //{
        //    Master_Node node = new Master_Node();

        //    Node_List.Add(node.Set_Node(pos, img));
        //}
        for (int i = 0; i < Node_List.Count; i++)
        {
            if (i == 0)
            {
                //Node_List[i].Set_Sub_N();
            }
            else
            {
                //Node_List[i].Set_Sub_P();
                //Node_List[i - 1].Set_Sub_N();
                Node_List[i - 1].Set_Next_Node(Node_List[i]);

            }

            if (i == Node_List.Count - 1)
            {
                //Node_List[0].Set_Sub_P();
                //Node_List[i].Set_Sub_N();
                Node_List[i].Set_Next_Node(Node_List[0]);
            }
        }

        creator._Path = null;
        creator.UpdateRoad();
        SectionDivide();
    }

    public void Center()
    {
        float max_x = 0, min_x = 0, max_y = 0, min_y = 0;

        for (int i = 0; i < Node_List.Count; i++)
        {
            if (Node_List[i].Main_Object.transform.localPosition.x >= max_x)
            {
                max_x = Node_List[i].Main_Object.transform.localPosition.x;
            }
            if (Node_List[i].Main_Object.transform.localPosition.x <= min_x)
            {
                min_x = Node_List[i].Main_Object.transform.localPosition.x;
            }
        }

        // y
        for (int i = 0; i < Node_List.Count; i++)
        {
            if (Node_List[i].Main_Object.transform.localPosition.y >= max_y)
            {
                max_y = Node_List[i].Main_Object.transform.localPosition.y;
            }
            if (Node_List[i].Main_Object.transform.localPosition.y <= min_y)
            {
                min_y = Node_List[i].Main_Object.transform.localPosition.y;
            }
        }

        float center_x = (max_x + min_x) / 2;
        float center_y = (max_y + min_y) / 2;

        for (int i = 0; i < Node_List.Count; i++)
        {
            Node_List[i].Main_Object.transform.localPosition -= new Vector3(center_x, center_y, 0);
            //if (center_x > 0)
            //{
            //    Node_List[i].Main_Object.transform.localPosition -= new Vector3(center_x, 0, 0);
            //}
            ////else if (center_x < 0)
            ////{
            ////    Node_List[i].Main_Object.transform.localPosition -= new Vector3(center_x, 0, 0);
            ////}


            //if (center_y > 0)
            //{
            //    Node_List[i].Main_Object.transform.localPosition -= new Vector3(0, center_y, 0);
            //}
            //else if (center_y < 0)
            //{
            //    Node_List[i].Main_Object.transform.localPosition -= new Vector3(0, center_y, 0);
            //}
        }

    }


    private void SectionDivide()
    {
        dividedSection = new List<Vector3[]>();

        for (int i = 0; i < Node_List.Count; i++)
        {
            Vector3[] p = new Vector3[4];
            p = GetPointsInSegment(i);
            Vector3[] Target_postions = new Vector3[DesignConstStorage.SectionSize];
            dividedSection.Add(Target_postions);

            float t = 0;

            for (int j = 0; j < DesignConstStorage.SectionSize; j++)
            {
                t = j / (float)DesignConstStorage.SectionSize;
                Target_postions[j] = EvaluateCubic(p[0], p[1], p[2], p[3], t);
            }
        }

        //for (int i = 0; i < dividedSection.Count; i++)
        //{
        //    var d = dividedSection[i];
        //    for (int j = 0; j < d.Length; j++)
        //    {
        //        Debug.LogError(d[j]);
        //    }
            
        //}
    }

    private Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }

    private Vector3 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        Vector2 p0 = EvaluateQuadratic(a, b, c, t);
        Vector2 p1 = EvaluateQuadratic(b, c, d, t);
        
        Vector3 pos = Vector2.Lerp(p0, p1, t);
        pos += Vector3.back;
        return pos;
    }

    private Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] {Node_List[i].transform.position,
            Node_List[i].Sub_Objects_N.transform.position, Node_List[(i + 1) % Node_List.Count].Sub_Objects_P.transform.position,
            Node_List[(i + 1) % Node_List.Count].transform.position };
    }

    private void Update()
    {
        if(Node_List != null)
        {
//#if UNITY_EDITOR
            creator.UpdateRoad();
//#endif
        }
    }

}
