using UnityEngine;

namespace Hexagon.ECS.Camera
{
    public interface ICameraComponent : IComponent
    {
        void LookAt(Vector3 pos);
    }
}