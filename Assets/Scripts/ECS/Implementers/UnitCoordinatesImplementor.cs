using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hexagon.Services.Map;
using Svelto.ECS;
using Debug = UnityEngine.Debug;

namespace Hexagon.ECS.Unit
{
    public class UnitCoordinatesImplementor : IImplementor, ICoordinatesComponent, IUnitAbilityComponent
    {
        public UnitCoordinatesImplementor(int x, int z)
        {
            coordinates = new HexCoordinates(x, z);
            direction = HexDirection.NE;            
        }

        public HexCoordinates coordinates { get; set; }

        public HexDirection direction { get; set; }


        public int currentAbility { get; set; }
        public List<HexCell> targets { get; set; }
    }
}