using Hexagon.Effects;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagon.ECS.Ability 
{
    public class AbilityImplementor : MonoBehaviour, IImplementor, IAbilityComponent, IAbilityRangeComponent,
        IAbilityAreaComponent, IAbilityEffectsComponent 
    {
        [SerializeField] string _spellName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxRange;
        [SerializeField] private int _area;
        [SerializeField] private List<Effect> _effects;

        void Awake()
        {
            apply = new DispatchOnSet<bool>(gameObject.GetInstanceID());
        }

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

        public List<Effect> effects
        {
            get
            {
                return _effects;
            }
        }

        public DispatchOnSet<bool> apply { get; private set; }
    }
}