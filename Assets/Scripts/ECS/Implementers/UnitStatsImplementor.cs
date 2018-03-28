using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hexagon.ECS.Components.Stats;
using Svelto.ECS;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hexagon.ECS.Unit {
    public class UnitStatsImplementor : MonoBehaviour, IImplementor, IHealthComponent 
    {

        private void Awake()
        {
            currentHealth = new DispatchOnChange<int>(gameObject.GetInstanceID());
            maxHealth = new DispatchOnChange<int>(gameObject.GetInstanceID());

        }


        public DispatchOnChange<int> currentHealth { get; private set; }
        public DispatchOnChange<int> maxHealth { get; private set; }
    }
}