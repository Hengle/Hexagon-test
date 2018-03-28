using System.Collections.Generic;

namespace Hexagon.ECS.Battle {
    public interface IBattleUnitsComponent {
        List<int> units { get; }
        int current { get; set; }
    }

    public struct TurnInfo
    {
        public int entityID { get; private set; }
        public TurnPhase turnPhase;

        public TurnInfo(int id) : this()
        {
            entityID = id;
            turnPhase = TurnPhase.Starting;
        }
    }

    public enum TurnPhase
    {
        Starting,
        Happening,
        Ending,
    }
}