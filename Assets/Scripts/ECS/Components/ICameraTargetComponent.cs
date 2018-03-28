using UnityEngine;

namespace Hexagon.ECS.Camera
{
    public interface ICameraTargetComponent
    {
        Vector3 position { get; }
        Vector3 rotation { get; set; }
    }
}