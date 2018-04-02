using Hexagon.ECS.Unit;
using Svelto.ECS;

namespace Hexagon.ECS.Player
{
    public class PlayerEntityView : EntityView
    {
        public ICoordinatesComponent coordinatesComponent;
        public IInputComponent inputComponent;
        public IUnitMovementComponent movementComponent;
        public ITransformComponent transformComponent;
        public ITurnComponent turnComponent;
    }
}