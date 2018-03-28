using UnityEngine;

namespace Hexagon.ECS
{
    public interface ITransformComponent : IComponent
    {
        Vector3 position { get; set; }
        Vector3 rotation { get; set; }
        Quaternion localRotation { get; set; }
    }

    public interface ICoordinatesComponent : IComponent
    {
        HexCoordinates coordinates { get; set; }
        HexDirection direction { get; set; }
    }

}