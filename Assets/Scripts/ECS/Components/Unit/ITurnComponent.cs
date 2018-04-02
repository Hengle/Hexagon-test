using Svelto.ECS;

namespace Hexagon.ECS.Unit
{
    public interface ITurnComponent : IComponent
    {
        int ctrCounter { get; set; }
        DispatchOnChange<int> moveCounter { get; }
        bool isInBattle { get; set; }
        DispatchOnSet<bool> isMyTurn { get; }
        DispatchOnSet<UnitTurnState> state { get; set; }
    }

    public enum UnitTurnState
    {
        Movement,
        AbilitySelected,
        AbilityTargeted,
    }
}