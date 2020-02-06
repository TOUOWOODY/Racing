using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RoadCreator : MonoBehaviour
{

    [Range(.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;
    public Path_Manager Path_Manager;
    public Draw_Line DL = null;
    public int Set_Start, segment_num;
    private Path path = null;
    private Vector3[] points2;
    public Path _Path
    {
        set
        {
            path = value;
        }
    }
    //private Mesh meshFilter = null;
    private MeshRenderer meshRenderer = null;
    
    public void UpdateRoad()
    {
        if (path == null)
        {
            RenderRoad();
        }
        else
        {
            //Debug.LogError("ReRenderRoad");
        }
    }

    public Mesh TossMesh()
    {
        Vector3[] points = path.CalculateEvenlySpacedPoints(spacing, Set_Start = 0, segment_num = path.NumSegments);
        
        return CreateRoadMesh(points, path.IsClosed);
    }

    public Vector3 TossMeshSize()
    {
        return meshRenderer.bounds.extents;
    }

    private void RenderRoad()
    {
        
        path = GetComponent<PathCreator>().path;
        //path.Points = Path_Manager.Node_List;
        Vector3[] points = path.CalculateEvenlySpacedPoints(spacing, Set_Start = 0, segment_num = path.NumSegments);

        DL.Line_Draw(points2 = path.CalculateEvenlySpacedPoints(spacing, Set_Start = 2, segment_num = path.NumSegments), path, path.IsClosed); //  Set_Start = 3 임시데이터
        //현재 널이 뜨는 상황이나 후에 DL을 옮기면 괜찮아질듯

        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points, path.IsClosed);
        int textureRepeat = Mathf.RoundToInt((tiling * points.Length * spacing * .05f) / 5); // tiling * points.Length * spacing * .05f    << 31인데 그림이 찌그러져 일단 8로 수정
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.mainTextureScale = new Vector3(1, textureRepeat);
    }

    Mesh CreateRoadMesh(Vector3[] points, bool isClosed)
    {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points.Length - 1) + ((isClosed) ? 2 : 0);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {

            Vector3 forward = Vector3.zero;
            if (i < points.Length - 1 || isClosed)
            {
                forward += points[(i) % points.Length] - points[i];
            }
            if (i > 0 || isClosed)
            {
                forward += points[i] - points[(i - 1 + points.Length) % points.Length];
            }

            forward.Normalize();
            Vector3 left = new Vector3(-forward.y, forward.x);

            verts[vertIndex] = (Vector3)points[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = (Vector3)points[i] - left * roadWidth * .5f;

            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new Vector3(0, v);
            uvs[vertIndex + 1] = new Vector3(1, v);

            if (i < points.Length - 1 || isClosed)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        return mesh;
    }

}