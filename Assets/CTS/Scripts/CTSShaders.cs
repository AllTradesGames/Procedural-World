using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CTS
{
    /// <summary>
    /// Get a CTS shader from a cached shader store
    /// </summary>
    public static class CTSShaders
    {
        private static Dictionary<string, Shader> m_shaderLookup = new Dictionary<string, Shader>();

        static CTSShaders()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Shader shader = Shader.Find(CTSConstants.CTSShaderLiteName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderLiteName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderBasicName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderBasicName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderBasicCutoutName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderBasicCutoutName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderAdvancedName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderAdvancedName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderAdvancedCutoutName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderAdvancedCutoutName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderTesselatedName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderTesselatedName, shader);
            }

            shader = Shader.Find(CTSConstants.CTSShaderTesselatedCutoutName);
            if (shader != null)
            {
                m_shaderLookup.Add(CTSConstants.CTSShaderTesselatedCutoutName, shader);
            }

            if (sw.ElapsedMilliseconds > 0)
            {
                //Debug.LogFormat("CTS located {0} CTS shaders in {1} ms.", m_shaderLookup.Count, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Get the designated shader.
        /// </summary>
        /// <param name="shaderType">Shader type</param>
        /// <returns>Shader or null if not found</returns>
        public static Shader GetShader(string shaderType)
        {
            Shader shader;
            if (m_shaderLookup.TryGetValue(shaderType, out shader))
            {
                return shader;
            }
            else
            {
                Debug.LogErrorFormat(
                    "Could not load CTS shader : {0}. Make sure you add your CTS shader to pre-loaded assets!",
                    shaderType);
                return null;
            }
        }
    }
}