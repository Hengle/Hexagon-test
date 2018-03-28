using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Battle {
    public class ActionBarImplementor : MonoBehaviour, IImplementor, IActionBarComponent {

        public int movesLeft { get; set; }

        void Awake()
        {

        }
    }
}