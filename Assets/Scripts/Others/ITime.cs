namespace Hexagon.ECS
{
    public interface ITime
    {
        float deltaTime { get; }
    }

    public class Time : ITime
    {
        public float deltaTime
        {
            get { return UnityEngine.Time.deltaTime; }
        }
    }
}