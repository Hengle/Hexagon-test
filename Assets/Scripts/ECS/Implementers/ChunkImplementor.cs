namespace Hexagon.ECS.Map
{
    public class ChunkImplementor : IImplementor, IChunkComponent
    {
        public ChunkImplementor(int ind, int terrain)
        {
            index = ind;
            terrainID = terrain;
        }

        public int index { get; set; }
        public int terrainID { get; private set; }
    }
}