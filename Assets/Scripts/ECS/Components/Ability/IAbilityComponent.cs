﻿using UnityEngine;

namespace Hexagon.ECS.Ability 
{
    public interface IAbilityComponent : IComponent 
    {
        string spellName { get; set; }
        Sprite icon { get; set; }
    }

    public struct AbilitySelectInfo {
        public int unitID { get; private set; }
        public int abilityID { get; private set; }

        public AbilitySelectInfo(int uid, int aid) : this() {
            unitID = uid;
            abilityID = aid;
        }
    }
}