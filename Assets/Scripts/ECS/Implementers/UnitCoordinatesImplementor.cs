using System;
using System.Collections.Generic;
using System.Diagnostics;
using Svelto.ECS;
using Debug = UnityEngine.Debug;

namespace Hexagon.ECS.Unit
{
    public class UnitCoordinatesImplementor : IImplementor, ICoordinatesComponent, IUnitStatsComponent
    {
        public UnitCoordinatesImplementor(int x, int z)
        {
            coordinates = new HexCoordinates(x, z);
            direction = HexDirection.NE;
            stats = new Dictionary<StatTypes, Stat>();
            foreach (StatTypes stat in Enum.GetValues(typeof(StatTypes)))
            {
                stats.Add(stat, new Stat());
            }
        }

        public HexCoordinates coordinates { get; set; }

        public HexDirection direction { get; set; }


        public Dictionary<StatTypes, Stat> stats { get; private set; }
    }
}