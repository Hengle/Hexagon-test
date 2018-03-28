using UnityEngine;

namespace Hexagon.ECS.Ability 
{
    public interface IAbilityAreaComponent : IComponent 
    {
        int area { get; set; }
    }
}