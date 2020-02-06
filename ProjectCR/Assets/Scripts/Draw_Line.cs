using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Draw_Line : MonoBehaviour
{
    [Range(.05f, 1.5f)]
    public float spacing = 1;
    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;


    public void Line_Draw(Vector3[] points,Path path, bool isClosed)
    {
        GetComponent<MeshFilter>().mesh = CreateRoadMesh(points, path.IsClosed);
    }

    Mesh CreateRoadMesh(Vector3[] points2, bool isClosed)
    {
        Vector3[] verts = new Vector3[points2.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points2.Length - 1) + (0);
        int[] tris = new int[numTris * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points2.Length; i++)
        {

            Vector3 forward = Vector3.zero;
            if (i < points2.Length - 1)
            {
                forward += points2[(i) % points2.Length] - points2[i];
            }
            if (i > 0)
            {
                forward += points2[i] - points2[(i - 1 + points2.Length) % points2.Length];
            }

            forward.Normalize();
            Vector3 left = new Vector3(-forward.y, forward.x);

            verts[vertIndex] = (Vector3)points2[i] + left * roadWidth * .5f;
            verts[vertIndex + 1] = (Vector3)points2[i] - left * roadWidth * .5f;

            float completionPercent = i / (float)(points2.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new Vector3(0, v);
            uvs[vertIndex + 1] = new Vector3(1, v);

            if (i < points2.Length - 1)
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
