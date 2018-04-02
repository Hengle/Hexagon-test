using System.Collections.Generic;
using Hexagon.Services.Map;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public interface IUnitAbilityComponent : IComponent
    {
        int currentAbility { get; set; }
        List<HexCell> targets { get; set; }
    }
}