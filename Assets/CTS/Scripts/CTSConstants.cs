using System;

namespace CTS
{
    /// <summary>
    /// CTS Constants
    /// </summary>
    public static class CTSConstants
    {
        /// <summary>
        /// Version information
        /// </summary>
        public static readonly int MajorVersion = 1;
        public static readonly int MinorVersion = 8;

        /// <summary>
        /// CTS Present define
        /// </summary>
        public static readonly string CTSPresentSymbol = "CTS_PRESENT";

        /// <summary>
        /// The shader being used
        /// </summary>
        public enum ShaderType { Unity, Basic, Advanced, Tesselation, Lite }

        /// <summary>
        /// Names of the various shaders
        /// </summary>
        public const string CTSShaderName = "CTS/CTS Terrain";
        public const string CTSShaderMeshBlenderName = "CTS/CTS_Model_Blend";
        public const string CTSShaderMeshBlenderAdvancedName = "CTS/CTS_Model_Blend_Advanced";
        public const string CTSShaderLiteName = "CTS/CTS Terrain Shader Lite";
        public const string CTSShaderBasicName = "CTS/CTS Terrain Shader Basic";
        public const string CTSShaderBasicCutoutName = "CTS/CTS Terrain Shader Basic CutOut";
        public const string CTSShaderAdvancedName = "CTS/CTS Terrain Shader Advanced";
        public const string CTSShaderAdvancedCutoutName = "CTS/CTS Terrain Shader Advanced CutOut";
        public const string CTSShaderTesselatedName = "CTS/CTS Terrain Shader Advanced Tess";
        public const string CTSShaderTesselatedCutoutName = "CTS/CTS Terrain Shader Advanced Tess CutOut";

        /// <summary>
        /// The shader mode being used
        /// </summary>
        public enum ShaderMode { DesignTime, RunTime }

        /// <summary>
        /// Occlusion Type being used
        /// </summary>
        public enum AOType { None, NormalMapBased, TextureBased}

        /// <summary>
        /// The size of the textures that will be used to generate atlases
        /// </summary>
        public enum TextureSize { Texture_64, Texture_128, Texture_256, Texture_512, Texture_1024, Texture_2048, Texture_4096, Texture_8192 }

        /// <summary>
        /// Get the size of the texture
        /// </summary>
        /// <param name="size">Texture size</param>
        /// <returns>Texture size or zero if invalid</returns>
        public static int GetTextureSize(TextureSize size)
        {
            switch (size)
            {
                case CTSConstants.TextureSize.Texture_64:
                    return 64;
                case CTSConstants.TextureSize.Texture_128:
                    return 128;
                case CTSConstants.TextureSize.Texture_256:
                    return 256;
                case CTSConstants.TextureSize.Texture_512:
                    return 512;
                case CTSConstants.TextureSize.Texture_1024:
                    return 1024;
                case CTSConstants.TextureSize.Texture_2048:
                    return 2048;
                case CTSConstants.TextureSize.Texture_4096:
                    return 4096;
                case CTSConstants.TextureSize.Texture_8192:
                    return 8192;
            }
            //Invalid setting passed in
            return 0;
        }

        /// <summary>
        /// Texture types
        /// </summary>
        public enum TextureType { Albedo, Normal, AmbientOcclusion, Height, Splat, Emission }

        /// <summary>
        /// Texture channels
        /// </summary>
        public enum TextureChannel { R, G, B, A }

        /// <summary>
        /// Flags used to decipher terrain changes
        /// </summary>
        [Flags]
        public enum TerrainChangedFlags
        {
            NoChange = 0,
            Heightmap = 1 << 0,
            TreeInstances = 1 << 1,
            DelayedHeightmapUpdate = 1 << 2,
            FlushEverythingImmediately = 1 << 3,
            RemoveDirtyDetailsImmediately = 1 << 4,
            WillBeDestroyed = 1 << 8,
        }
    }
}

