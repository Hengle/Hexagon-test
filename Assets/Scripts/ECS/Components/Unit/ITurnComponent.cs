using Svelto.ECS;

namespace Hexagon.ECS.Unit
{
    public interface ITurnComponent : IComponent
    {
        int ctrCounter { get; set; }
        DispatchOnChange<int> moveCounter { get; }
        bool isInBattle { get; set; }
        DispatchOnSet<bool> isMyTurn { get; }
        UnitTurnState state { get; set; }
        int currentAbility { get; set; }
    }

    public enum UnitTurnState
    {
        Movement,
        Ability,
    }
}