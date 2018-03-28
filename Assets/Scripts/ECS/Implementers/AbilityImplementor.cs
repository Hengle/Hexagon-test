using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Ability 
{
    public class AbilityImplementor : MonoBehaviour, IImplementor, IAbilityComponent, IAbilityRangeComponent, IAbilityAreaComponent 
    {
        [SerializeField] string _spellName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxRange;
        [SerializeField] private int _area;

        public string spellName
        {
            get { return _spellName; }
            set { _spellName = value; }
        }

        public Sprite icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public int maxRange
        {
            get { return _maxRange; }
            set { _maxRange = value; }
        }

        public int area
        {
            get { return _area - 1; }
            set { _area = value; }
        }
    }
}