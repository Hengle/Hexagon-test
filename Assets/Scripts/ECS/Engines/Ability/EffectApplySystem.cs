using System.Collections;
using System.Collections.Generic;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Ability
{
    public class EffectApplySystem : SingleEntityViewEngine<AbilityEntityView>
    {

        protected override void Add(AbilityEntityView entityView)
        {
            entityView.effectsComponent.apply.NotifyOnValueSet(OnApplyEffects);
        }

        protected override void Remove(AbilityEntityView entityView)
        {
            entityView.effectsComponent.apply.StopNotify(OnApplyEffects);
        }

        void OnApplyEffects(int senderId, bool apply)
        {

        }
    }
}

