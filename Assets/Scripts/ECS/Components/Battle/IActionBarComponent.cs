namespace Hexagon.ECS.Battle {
    public interface IActionBarComponent : IComponent
    {
        int movesLeft { get; set; }
    }
}