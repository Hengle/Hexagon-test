using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public interface IInputComponent
    {
        DispatchOnSet<HexCoordinates> selection { get; set; }
        DispatchOnSet<HexCoordinates> cellClick { get; set; }
        Ray camRay { get; set; }
        DispatchOnSet<bool> pass { get; }
        DispatchOnSet<int> abilitySelected { get; }
        DispatchOnSet<bool> cancel { get; }
    }
}