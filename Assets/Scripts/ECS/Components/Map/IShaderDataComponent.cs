using UnityEngine;

namespace Hexagon.ECS.Map
{
    public interface IShaderDataComponent : IComponent
    {
        Texture2D cellTexture { get; set; }
        Color32[] cellTextureData { get; set; }
    }
}