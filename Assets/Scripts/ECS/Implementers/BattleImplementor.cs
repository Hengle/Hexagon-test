using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Battle
{
    public class BattleImplementor : IImplementor, IBattleStateComponent, IBattleUnitsComponent
    {
        public DispatchOnSet<BattleState> state { get; private set; }

        public List<int> units { get; private set; }
        public int current { get; set; }

        public BattleImplementor()
        {
            state = new DispatchOnSet<BattleState>();
            units = new List<int>(); 
        }


        
    }
}