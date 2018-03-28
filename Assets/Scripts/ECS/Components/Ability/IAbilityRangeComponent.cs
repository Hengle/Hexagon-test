using UnityEngine;

namespace Hexagon.ECS.Ability 
{
    public interface IAbilityRangeComponent : IComponent 
    {
        int maxRange { get; set; }
    }
}