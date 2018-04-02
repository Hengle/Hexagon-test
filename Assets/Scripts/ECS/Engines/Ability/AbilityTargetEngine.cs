using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Battle;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.UI;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Ability {
    public class AbilityTargetEngine : IQueryingEntityViewEngine, IStep<AbilitySelectInfo> {

        public AbilityTargetEngine()
        {

        }


        public IEntityViewsDB entityViewsDB { private get; set; }

        public void Ready()
        {
        }

        public void Step(ref AbilitySelectInfo token, int condition) 
        {
            
        }

    }

}
