using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public interface IUnitMovementComponent
    {
        bool isWalking { get; set; }
        int move(Vector3 target);
        int rotate(Vector3 localEuler);

        int maxMove { get; set; }
    }
}