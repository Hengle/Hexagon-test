using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Unit {

    public interface IUnitStatsComponent : IComponent {
        Dictionary<StatTypes, Stat> stats { get; }
    }

    public struct Stat
    {

        public Stat(int value)
        {
            raw = value;
            total = value;
        }

       public int raw;
       public int total;
    

    }

}