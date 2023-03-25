using Asteroids.Data;
using Asteroids.Tools;
using UnityEngine;

namespace Asteroids.Tools
{
    public class ConvertPolygonToMesh : MonoBehaviour
    {
        [SerializeField] PowerDataSO Data;
        [SerializeField] int type;
        [SerializeField] float radius;
        [SerializeField] PolygonCollider2D Polygon;
        [SerializeField] MeshFilter Mesh;
        [SerializeField] MeshFilter circleMesh;

        void Start()
        {
            AMeshTools.CreateCircleMesh(circleMesh, radius, 20);
            AMeshTools.CreateMeshWithMassCenter(Data.Get(type).shape, transform.localScale, Mesh.mesh);
        }
    }
}