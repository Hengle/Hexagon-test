using Svelto.ECS;

namespace Hexagon.ECS.Camera
{
    public class CameraEntityView : EntityView
    {
        public ICameraComponent cameraComponent;
        public ITransformComponent transformComponent;
    }
}