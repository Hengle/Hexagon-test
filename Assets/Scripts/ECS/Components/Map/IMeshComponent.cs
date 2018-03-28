using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Map
{
    public interface IMeshComponent : IComponent
    {
        bool useCollider { get; }
        bool useCellData { get; }


        List<Vector3> vertices { get; set; }
        List<Vector3> cellIndices { get; set; }
        List<Color> cellWeights { get; set; }
        List<Vector2> uvs { get; set; }
        List<Vector2> uvs2 { get; set; }
        List<int> triangles { get; set; }
        Mesh mesh { get; }
        MeshCollider meshCollider { get; }

        void clear();

        void setVertices(List<Vector3> vertices);
    }
}