using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NationsWars.Game
{
    /// <summary>Flat-color materials that work on the built-in pipeline now and on URP once it is added.</summary>
    public static class MaterialUtil
    {
        static readonly Dictionary<Color, Material> Cache = new Dictionary<Color, Material>();

        public static Material Get(Color color)
        {
            Material m;
            if (Cache.TryGetValue(color, out m) && m != null) return m;
            m = Create(color);
            Cache[color] = m;
            return m;
        }

        public static Material Create(Color color)
        {
            Shader shader = GraphicsSettings.currentRenderPipeline != null
                ? Shader.Find("Universal Render Pipeline/Lit")
                : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            return new Material(shader) { color = color };
        }
    }
}
