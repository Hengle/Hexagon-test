using UnityEngine;
using Hexagon.Effects;
using System.Collections.Generic;
using Svelto.ECS;

namespace Hexagon.ECS.Ability
{
    public interface IAbilityEffectsComponent : IComponent
    {
        List<Effect> effects { get; }
        DispatchOnSet<bool> apply { get; }
    }
}