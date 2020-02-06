using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{

    [HideInInspector]
    public Path path;
    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;

    public Path_Manager path_Manager;

    public void CreatePath()
    {

        path_Manager.Initialize();
        path = new Path();
        path.Points = path_Manager.Node_List;
    }

    void Reset()
    {
        CreatePath();
    }
}