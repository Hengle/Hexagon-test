using Hexagon.ECS.Unit;
using Svelto.ECS;

namespace Hexagon.ECS.Engines.Unit
{
    public class UnitAnimationEngine : SingleEntityViewEngine<UnitEntityView>, IQueryingEntityViewEngine
    {
        public void Ready()
        {
        }

        public IEntityViewsDB entityViewsDB { set; private get; }

        protected override void Add(UnitEntityView entityView)
        {
        }

        protected override void Remove(UnitEntityView entityView)
        {
        }
    }
}