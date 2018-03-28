namespace Hexagon.ECS.Unit
{
    public interface IUnitAnimationComponent
    {
        string trigger { set; }
        void setBool(string name, bool value);
    }
}