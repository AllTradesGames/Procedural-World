using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
#endif

namespace CTS
{
    /// <summary>
    /// Controller coordinator for demo scenes
    /// </summary>
    public class CTSDemoController : MonoBehaviour
    {
        [Header("Target")]
        public GameObject m_target;

        [Header("Walk Controller")]
        public CTSWalk m_walkController;
        private CharacterController m_characterController;

        [Header("Fly Controller")]
        public CTSFly m_flyController;

        [Header("Look Controller")]
        public CTSLook m_lookController;

        [Header("Profiles")]
        public CTSProfile m_unityProfile;
        public CTSProfile m_liteProfile;
        public CTSProfile m_basicProfile;
        public CTSProfile m_advancedProfile;
        public CTSProfile m_tesselatedProfile;

        [Header("UX Text")]
        public Text m_mode;
        public Text m_readme;
        public Text m_instructions;

        [Header("Post FX")]
        public ScriptableObject m_postFX;
        private Component m_postProcessingComponent;

        void Awake()
        {
            //Target
            if (m_target == null)
            {
                m_target = Camera.main.gameObject;
            }

            #region PostFX
            //See if we can set up post processing for better lighting
            try
            {
                if (m_postFX != null)
                {
                    Camera camera = Camera.main;
                    if (camera == null)
                    {
                        camera = FindObjectOfType<Camera>();
                    }
                    if (camera != null)
                    {
                        Type postProcessingType = GetType("UnityEngine.PostProcessing.PostProcessingBehaviour");
                        if (postProcessingType != null)
                        {
                            GameObject cameraObj = camera.gameObject;

                            m_postProcessingComponent = cameraObj.GetComponent(postProcessingType);
                            if (m_postProcessingComponent == null)
                            {
                                m_postProcessingComponent = cameraObj.AddComponent(postProcessingType);
                            }
                            if (m_postProcessingComponent != null)
                            {
                                FieldInfo fi = postProcessingType.GetField("profile", BindingFlags.Public | BindingFlags.Instance);
                                if (fi != null)
                                {
                                    fi.SetValue(m_postProcessingComponent, m_postFX);
                                }
                                ((MonoBehaviour) m_postProcessingComponent).enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("Failed to set up post fx.");
            }
            #endregion

            #region Lighting check
            #if UNITY_EDITOR
            if (m_readme != null)
            {
                string readme = "";
                bool needLightingUpdate = false;

                if (PlayerSettings.colorSpace != ColorSpace.Linear)
                {
                    needLightingUpdate = true;
                }

                if (!needLightingUpdate)
                {
                    var tier1 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier1);
                    if (tier1.renderingPath != RenderingPath.DeferredShading)
                    {
                        needLightingUpdate = true;
                    }
                }

                if (!needLightingUpdate)
                {
                    var tier2 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier2);
                    if (tier2.renderingPath != RenderingPath.DeferredShading)
                    {
                        needLightingUpdate = true;
                    }
                }

                if (!needLightingUpdate)
                {
                    var tier3 = EditorGraphicsSettings.GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, GraphicsTier.Tier3);
                    if (tier3.renderingPath != RenderingPath.DeferredShading)
                    {
                        needLightingUpdate = true;
                    }
                }

                if (needLightingUpdate)
                {
                    readme = "Instructions : Lighting incorrect";
                }

                if (m_postProcessingComponent == null)
                {
                    if (readme.Length == 0)
                    {
                        readme = "Instructions : Post FX missing.";
                    }
                    else
                    {
                        readme += ", Post FX missing";
                    }
                }

                if (readme.Length > 0)
                {
                    readme += ". Please read CTS_Demo_ReadMe to fix!";
                }
                m_readme.text = readme;
            }
#endif
            #endregion

            #region Controller setup

            //Fly controller
            if (m_flyController == null)
            {
                m_flyController = m_target.GetComponent<CTSFly>();
            }
            if (m_flyController == null)
            {
                m_flyController = m_target.AddComponent<CTSFly>();
            }
            m_flyController.enabled = false;

            //Character controller
            if (m_characterController == null)
            {
                m_characterController = m_target.GetComponent<CharacterController>();
            }
            if (m_characterController == null)
            {
                m_characterController = m_target.AddComponent<CharacterController>();
                m_characterController.height = 4f;
            }
            m_characterController.enabled = false;

            //Walk controller
            if (m_walkController == null)
            {
                m_walkController = m_target.GetComponent<CTSWalk>();
            }
            if (m_walkController == null)
            {
                m_walkController = m_target.AddComponent<CTSWalk>();
                m_walkController.m_controller = m_characterController;
            }
            m_walkController.enabled = false;

            //Look controller
            if (m_lookController == null)
            {
                m_lookController = m_target.GetComponent<CTSLook>();
            }
            if (m_lookController == null)
            {
                m_lookController = m_target.AddComponent<CTSLook>();
                m_lookController._playerRootT = m_target.transform;
                m_lookController._cameraT = m_target.transform;
            }
            m_lookController.enabled = false;
            #endregion

            #region Instructions
            if (m_instructions != null)
            {
                string commands = "";
                if (m_unityProfile != null)
                {
                    commands += "Controls: 1. Unity";
                }
                if (m_liteProfile != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 2. Lite";
                    }
                    else
                    {
                        commands = "Controls: 2. Lite";
                    }
                }
                if (m_basicProfile != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 3. Basic";
                    }
                    else
                    {
                        commands = "Controls: 3. Basic";
                    }
                }
                if (m_advancedProfile != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 4. Advanced";
                    }
                    else
                    {
                        commands = "Controls: 4. Advanced";
                    }
                }
                if (m_tesselatedProfile != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 5. Tesselated";
                    }
                    else
                    {
                        commands = "Controls: 5. Tesselated";
                    }
                }
                if (m_flyController != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 6. Fly";
                    }
                    else
                    {
                        commands = "Controls: 6. Fly";
                    }
                }
                if (m_walkController != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", 7. Walk";
                    }
                    else
                    {
                        commands = "Controls: 7. Walk";
                    }
                }
                if (m_postProcessingComponent != null)
                {
                    if (commands.Length > 0)
                    {
                        commands += ", P. Post FX";
                    }
                    else
                    {
                        commands = "Controls: P. Post FX";
                    }
                }
                if (commands.Length > 0)
                {
                    commands += ", ESC. Exit.";
                }
                else
                {
                    commands = "Controls: ESC. Exit.";
                }
                m_instructions.text = commands;
            }
            #endregion

            //Start in basic mode
            SelectBasic();

            //At home
            if (m_flyController != null)
            {
                m_flyController.enabled = false;
            }
            if (m_walkController != null)
            {
                m_walkController.enabled = false;
            }
            if (m_characterController != null)
            {
                m_characterController.enabled = false;
            }
            if (m_lookController != null)
            {
                m_lookController.enabled = false;
            }
        }

        public void SelectUnity()
        {
            if (m_unityProfile != null)
            {
                CTSTerrainManager.Instance.BroadcastProfileSelect(m_unityProfile);
                if (m_mode != null)
                {
                    m_mode.text = "Unity";
                }
            }
        }

        public void SelectLite()
        {
            if (m_liteProfile != null)
            {
                CTSTerrainManager.Instance.BroadcastProfileSelect(m_liteProfile);
                if (m_mode != null)
                {
                    m_mode.text = "Lite";
                }
            }
        }

        public void SelectBasic()
        {
            if (m_basicProfile != null)
            {
                CTSTerrainManager.Instance.BroadcastProfileSelect(m_basicProfile);
                if (m_mode != null)
                {
                    m_mode.text = "Basic";
                }
            }
        }

        public void SelectAdvanced()
        {
            if (m_advancedProfile != null)
            {
                CTSTerrainManager.Instance.BroadcastProfileSelect(m_advancedProfile);
                if (m_mode != null)
                {
                    m_mode.text = "Advanced";
                }
            }
        }

        public void SelectTesselated()
        {
            if (m_tesselatedProfile != null)
            {
                CTSTerrainManager.Instance.BroadcastProfileSelect(m_tesselatedProfile);
                if (m_mode != null)
                {
                    m_mode.text = "Tesselated";
                }
            }
        }

        public void Fly()
        {
            if (m_flyController != null)
            {
                if (!m_flyController.isActiveAndEnabled)
                {
                    if (m_characterController != null)
                    {
                        m_characterController.enabled = false;
                    }
                    if (m_walkController != null)
                    {
                        if (m_walkController.isActiveAndEnabled)
                        {
                            m_walkController.enabled = false;
                        }
                    }
                    if (m_lookController != null)
                    {
                        m_lookController.enabled = true;
                    }
                    m_flyController.enabled = true;
                }
            }
        }

        public void Walk()
        {
            if (m_walkController != null)
            {
                if (!m_walkController.isActiveAndEnabled)
                {
                    if (m_flyController != null)
                    {
                        if (m_flyController.isActiveAndEnabled)
                        {
                            m_flyController.enabled = false;
                        }
                    }
                    if (m_characterController != null)
                    {
                        m_characterController.enabled = true;
                    }
                    if (m_lookController != null)
                    {
                        m_lookController.enabled = true;
                    }
                    m_walkController.enabled = true;
                }
            }
        }

        /// <summary>
        /// Toggle Post FX if they exist
        /// </summary>
        public void PostFX()
        {
            if (m_postProcessingComponent != null)
            {
                if (((MonoBehaviour) m_postProcessingComponent).isActiveAndEnabled)
                {
                    ((MonoBehaviour)m_postProcessingComponent).enabled = false;
                }
                else
                {
                    ((MonoBehaviour)m_postProcessingComponent).enabled = true;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectUnity();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectLite();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectBasic();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectAdvanced();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectTesselated();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Fly();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Walk();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                PostFX();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }

        /// <summary>
        /// Get the specified type if it exists
        /// </summary>
        /// <param name="TypeName">Name of the type to load</param>
        /// <returns>Selected type or null</returns>
        public static Type GetType(string TypeName)
        {
            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {
                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly == null)
                        return null;

                    // Ask that assembly to return the proper Type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
                catch (Exception)
                {
                    //Debug.Log("Unable to load assemmbly : " + ex.Message);
                }
            }

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type
            var currentAssembly = Assembly.GetCallingAssembly();
            {
                // Load the referenced assembly
                if (currentAssembly != null)
                {
                    // See if that assembly defines the named type
                    type = currentAssembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }

            }

            //All loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int asyIdx = 0; asyIdx < assemblies.GetLength(0); asyIdx++)
            {
                type = assemblies[asyIdx].GetType(TypeName);
                if (type != null)
                {
                    return type;
                }
            }

            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                // Load the referenced assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    // See if that assembly defines the named type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            // The type just couldn't be found...
            return null;
        }
    }
}
