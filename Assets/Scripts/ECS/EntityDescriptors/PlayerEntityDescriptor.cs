using Hexagon.ECS.Camera;
using Hexagon.ECS.Player;
using Svelto.ECS;

namespace Hexagon.ECS.Unit
{
    internal class
        PlayerEntityDescriptor : GenericEntityDescriptor<UnitEntityView, PlayerEntityView, CameraTargetEntityView>
    {
    }
}