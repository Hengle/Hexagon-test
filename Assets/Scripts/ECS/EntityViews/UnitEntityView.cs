using Hexagon.ECS.Components.Stats;
using Svelto.ECS;

namespace Hexagon.ECS.Unit
{
    public class UnitEntityView : EntityView
    {
        public ICoordinatesComponent coordinatesComponent;
        public ITransformComponent transformComponent;
        public ITurnComponent turnComponent;
        public IInputComponent inputComponent;
        public IUnitMovementComponent movementComponent;
        public IHealthComponent healthComponent;
        public IUnitAbilityComponent abilityComponent;
    }
}