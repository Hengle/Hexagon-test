namespace Hexagon.ECS.Map
{
    public interface IChunkComponent : IComponent
    {
        int index { get; set; }
        int terrainID { get; }
    }
}