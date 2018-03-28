using Svelto.ECS;

namespace Hexagon.ECS.Battle
{
    public interface IBattleStateComponent : IComponent
    {
        DispatchOnSet<BattleState> state { get; }
    }
}