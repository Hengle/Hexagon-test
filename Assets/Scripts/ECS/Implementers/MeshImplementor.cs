using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Map
{
    public class MeshImplementor : MonoBehaviour, IImplementor, IMeshComponent
    {
        public bool CellData;

        public bool Collider;


        public bool useCollider
        {
            get { return Collider; }
        }

        public bool useCellData
        {
            get { return CellData; }
        }

        public List<Vector3> vertices { get; set; }
        public List<Vector3> cellIndices { get; set; }
        public List<Color> cellWeights { get; set; }
        public List<Vector2> uvs { get; set; }
        public List<Vector2> uvs2 { get; set; }
        public List<int> triangles { get; set; }

        public Mesh mesh { get; private set; }

        public MeshCollider meshCollider { get; private set; }

        public void clear()
        {
            mesh.Clear();
        }

        public void setVertices(List<Vector3> vertices)
        {
            mesh.SetVertices(vertices);
        }

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Hex Mesh";

            if (Collider) meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }
}