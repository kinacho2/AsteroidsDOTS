using Asteroids.Tools;
using UnityEngine;

public class ConvertPolygonToMesh : MonoBehaviour
{
    [SerializeField] PolygonCollider2D Polygon;
    [SerializeField] MeshFilter Mesh;
    void Start()
    {
        CreateMeshWithMassCenter(Polygon.points, Mesh.mesh);
    }

    void CreateMeshWithMassCenter(Vector2[] points, Mesh mesh)
    {
        Vector2 center = Vector3.zero;
        foreach (var point in points)
        {
            center += point;
        }
        center /= points.Length;

        Vector3[] vertices = new Vector3[points.Length + 1];
        Vector2[] uvs = new Vector2[points.Length + 1];
        int[] indices = new int[(points.Length) * 3];

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];
            uvs[i] = Vector2.zero;
        }
        vertices[points.Length] = center;
        uvs[points.Length] = Vector2.one;

        for (int i = 0; i < points.Length; i++)
        {
            indices[i * 3] = i;
            indices[i * 3 + 1] = points.Length;
            indices[i * 3 + 2] = (i + 1) % points.Length;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = indices;
    }

    void CreateMesh(Vector2[] points, Mesh mesh)
    {
        
        Vector3[] vertices = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[points.Length * 2];
        int[] indices = new int[(points.Length) * 3];

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];

            var point1 = points[i];
            var point2 = points[(i + 1) % points.Length];

            var diff = (point2 - point1);
            var dir = diff.normalized;
            var dir2 = AGeometry.RotateZ(dir, Mathf.PI*0.5f);

            vertices[points.Length + i] = point1 + dir * diff.magnitude * 0.5f + dir2;

            uvs[i] = Vector2.zero;
            uvs[points.Length + i] = Vector2.one;
        }

        for (int i = 0; i < points.Length; i++)
        {
            //int idx0 = i * 3;
            //int idx1 = i * 3 + 1;
            //int idx2 = i * 3 + 2;
            indices[i * 3] = i;
            indices[i * 3 + 1] = points.Length + i;
            indices[i * 3 + 2] = (i + 1) % points.Length;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = indices;
    }

    void CreateMeshWithCenter(Vector2[] points, Mesh mesh)
    {
        Vector2[] pointsCenter = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsCenter[i] = points[i] * 0.9f;
        }

        Vector3[] vertices = new Vector3[points.Length * 2];
        
        Vector2[] uvs = new Vector2[points.Length * 2];
        int[] indices = new int[(points.Length) * 6];

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];

            var point1 = points[i];
            var point2 = points[(i + 1) % points.Length];

            vertices[points.Length + i] = pointsCenter[i];

            uvs[i] = Vector2.zero;
            uvs[points.Length + i] = Vector2.one;
        }

        for (int i = 0; i < points.Length; i++)
        {
            //int idx0 = i * 3;
            //int idx1 = i * 3 + 1;
            //int idx2 = i * 3 + 2;
            indices[i * 3] = i;
            indices[i * 3 + 1] = points.Length + i;
            indices[i * 3 + 2] = (i + 1) % points.Length;

            indices[(points.Length + i) * 3] = points.Length + i;
            indices[(points.Length + i) * 3 + 1] = points.Length + (i + 1) % points.Length;
            indices[(points.Length + i) * 3 + 2] = (i + 1) % points.Length;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = indices;
    }
}
