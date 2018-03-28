using Svelto.ECS;

namespace Hexagon.ECS.Components.Stats
{
    public interface IHealthComponent : IComponent
    {
        DispatchOnChange<int> currentHealth { get; }
        DispatchOnChange<int> maxHealth { get; }
    }
}