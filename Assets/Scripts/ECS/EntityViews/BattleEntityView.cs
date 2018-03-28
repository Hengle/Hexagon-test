using Svelto.ECS;

namespace Hexagon.ECS.Battle
{
    public class BattleEntityView : EntityView
    {
        public IBattleStateComponent battleStateComponent;
        public IBattleUnitsComponent unitsComponent;
    }
}