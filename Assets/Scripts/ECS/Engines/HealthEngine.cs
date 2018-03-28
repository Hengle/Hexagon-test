using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Battle;
using Hexagon.ECS.Others;
using Hexagon.Services.UI;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit {
    public class HealthEngine : SingleEntityViewEngine<UnitEntityView> {


        protected override void Add(UnitEntityView entityView) {
            InitializeHealth(entityView);
        }

        protected override void Remove(UnitEntityView entityView) {
        }

        void InitializeHealth(UnitEntityView entityView)
        {
            entityView.healthComponent.maxHealth.value = 100;
            entityView.healthComponent.currentHealth.value = Random.Range(30, 100);
        }

    }

}
