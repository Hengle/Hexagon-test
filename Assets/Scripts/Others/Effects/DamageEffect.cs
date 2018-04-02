using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.Effects
{
    public class DamageEffect : Effect
    {
        [SerializeField] private int _value;

        public int value
        {
            get
            {
                return _value;
            }
            private set
            {
                _value = value;
            }
        }

    }
}


