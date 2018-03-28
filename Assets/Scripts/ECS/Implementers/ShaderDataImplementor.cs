using UnityEngine;

namespace Hexagon.ECS.Map
{
    public class ShaderDataImplementor : IImplementor, IShaderDataComponent
    {
        public Texture2D cellTexture { get; set; }
        public Color32[] cellTextureData { get; set; }
    }
}