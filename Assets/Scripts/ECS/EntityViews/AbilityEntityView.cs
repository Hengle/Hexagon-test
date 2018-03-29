using Svelto.ECS;

namespace Hexagon.ECS.Ability
{
    public class AbilityEntityView : EntityView
    {
        public IAbilityComponent abilityComponent;
        public IAbilityRangeComponent rangeComponent;
        public IAbilityAreaComponent areaComponent;
        public IAbilityEffectsComponent effectsComponent;
    }
}