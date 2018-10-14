#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Reflection;

namespace Quixel
{
    public class MegascansImporterWindow : EditorWindow
    {
        static private int texPack;
        static private int texPackUpdate;
        static private string[] texPacking = new string[]
        {
            "Metallic",
            "Specular",
        };
        static private int dispType;
        static private int dispTypeUpdate;
        static private string[] dispTypes = new string[]
        {
            "None",
            "Vertex",
            "Pixel",
        };
        static private int shaderType;
        static private int shaderTypeUpdate;
        static private string[] shaderTypes = new string[]
        {
            "HDRenderPipeline",
            "Lightweight",
            "Legacy",
            "Auto-Detect",
        };

        static private string path;
        static private string prefix;
        static private string suffix;
        static private string pathUpdate;
        static private string prefixUpdate;
        static private string suffixUpdate;

        static private Texture2D MSLogo;
        static private GUIStyle MSLogoStyle;
        static private Texture2D MSBackground;
        static private GUIStyle MSField;
        static private GUIStyle MSPopup;
        static private GUIStyle MSText;
        static private GUIStyle MSCheckBox;
        static private GUIStyle MSHelpStyle;
        static private GUIStyle MSConnectStyle;
        static private bool connection;
        static private bool connectionUpdate;

        static private bool SuperHD;

        static private Vector2 size;
        static private Vector2 logoSize;
        static private Vector2 textSize;
        static private Vector2 fieldSize;
        static private Rect mainSize;
        static private Rect connectionLoc;

        [MenuItem("Assets/Quixel/Megascans Importer")]
        public static void Init()
        {
            MegascansImporterWindow window = (MegascansImporterWindow)EditorWindow.GetWindow(typeof(MegascansImporterWindow));
            window.maxSize = size;
            window.minSize = size;
            window.Show();
        }

        void OnGUI()
        {
            GUI.DrawTexture(mainSize, MSBackground, ScaleMode.StretchToFill);

            GUILayout.BeginHorizontal();

            if(GUILayout.Button(MSLogo, MSLogoStyle, GUILayout.Height(logoSize.y), GUILayout.Width(logoSize.x)))
            {
                Application.OpenURL("Https://Megascans.se");
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Workflow", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            texPack = EditorGUILayout.Popup(texPack, texPacking, MSPopup, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Displacement", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            dispType = EditorGUILayout.Popup(dispType, dispTypes, MSPopup, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Shader Type", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            shaderType = EditorGUILayout.Popup(shaderType, shaderTypes, MSPopup, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Import Path", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            path = EditorGUILayout.DelayedTextField(path, MSField, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Asset Prefix", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            prefix = EditorGUILayout.DelayedTextField(prefix, MSField, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("Asset Suffix", MSText, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            suffix = EditorGUILayout.DelayedTextField(suffix, MSField, GUILayout.Height(fieldSize.y), GUILayout.Width(fieldSize.x));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            connection = EditorGUI.Toggle(connectionLoc, connection, MSCheckBox);
            GUILayout.Box("Enable Live Link", MSConnectStyle, GUILayout.Height(textSize.y), GUILayout.Width(textSize.x));
            if (GUILayout.Button("Help...", MSHelpStyle))
            {
                Application.OpenURL("https://docs.google.com/document/d/17FmQzTxo63NIvGkRcfVfLtp73GSfBBvZmlq3v6C-9nY/edit?usp=sharing");
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            if(! MSLogo)
            {
                InitStyle();
                Repaint();
            }
        }

        void OnEnable()
        {
            SuperHD = false;
            if (Display.main.systemHeight > 2100)
            {
                SuperHD = true;
            }
            size = SuperHD ? new Vector2(505, 615) : new Vector2(268, 317);
            textSize = SuperHD ? new Vector2(176, 54) : new Vector2(76, 30);
            fieldSize = SuperHD ? new Vector2(290, 54) : new Vector2(152, 30);
            mainSize = SuperHD ? new Rect(0, 0, 517, 620) : new Rect(0, 0, 268, 325);
            connectionLoc = SuperHD ? new Rect(25, 553, 32, 32) : new Rect(13,293,17,17);
            logoSize = SuperHD ? new Vector2(64, 64) : new Vector2(34, 34);
            InitStyle();
            GetDefaults();
            Repaint();
        }

        static void GetDefaults()
        {
            if (EditorPrefs.GetBool("QuixelDefaultOverride"))
            {
                path = EditorPrefs.GetString("QuixelDefaultPath");
                prefix = EditorPrefs.GetString("QuixelDefaultPrefix");
                suffix = EditorPrefs.GetString("QuixelDefaultSuffix");
                dispType = EditorPrefs.GetInt("QuixelDefaultDisplacement");
                texPack = EditorPrefs.GetInt("QuixelDefaultTexPacking");
                shaderType = EditorPrefs.GetInt("QuixelDefaultShader");
                connection = EditorPrefs.GetBool("QuixelDefaultConnection");
            }
            else
            {
                texPack = 0;
                shaderType = 3;
                dispType = 0;
                path = "Quixel/Megascans/";
                prefix = "";
                connection = false;
            }
            pathUpdate = path;
            prefixUpdate = prefix;
            suffixUpdate = suffix;
            dispTypeUpdate = dispType;
            texPackUpdate = texPack;
            shaderTypeUpdate = shaderType;
            connectionUpdate = connection;
        }

        void SaveDefaults()
        {
            EditorPrefs.SetBool("QuixelDefaultOverride", true);
            EditorPrefs.SetString("QuixelDefaultPath", path);
            EditorPrefs.SetString("QuixelDefaultPrefix", prefix);
            EditorPrefs.SetString("QuixelDefaultSuffix", suffix);
            EditorPrefs.SetInt("QuixelDefaultDisplacement", dispType);
            EditorPrefs.SetInt("QuixelDefaultTexPacking", texPack);
            EditorPrefs.SetInt("QuixelDefaultShader", shaderType);
            EditorPrefs.SetBool("QuixelDefaultConnection", connection);
            pathUpdate = path;
            prefixUpdate = prefix;
            suffixUpdate = suffix;
            dispTypeUpdate = dispType;
            texPackUpdate = texPack;
            shaderTypeUpdate = shaderType;
            connectionUpdate = connection;
            if( ! connection)
            {
                MegascansBridgeLink.EndServer();
            }
            if( connection)
            {
                MegascansBridgeLink.StartServer();
            }
        }

        void ConstructPopUp()
        {
            MSPopup = new GUIStyle();
            MSPopup.normal.textColor = new Color(1.0f, 1.0f, 1.0f);
            MSPopup.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/Popup_Background.png");
            MSPopup.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Quixel/Scripts/Fonts/SourceSansPro-Regular.ttf");
            MSPopup.fontSize = SuperHD ? 24 : 13;
            MSPopup.padding = SuperHD ? new RectOffset(20, 0, 10, 0) : new RectOffset(10, 5, 7, 4);
            MSPopup.margin = SuperHD ? new RectOffset(0, 20, 13, 7) : new RectOffset(0, 10, 6, 5);
        }

        void ConstructText()
        {
            MSText = new GUIStyle();
            MSText.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
            MSText.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Quixel/Scripts/Fonts/SourceSansPro-Regular.ttf");
            MSText.fontSize = SuperHD ? 24 : 13;
            MSText.padding = SuperHD ? new RectOffset(5,0,10,0) : new RectOffset(5,5,7,4);
            MSText.margin = SuperHD ? new RectOffset(20, 0, 13, 7) : new RectOffset(10,20,6,5);
        }

        void ConstructBackground()
        {
            MSBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/Background.png");
        }

        void ConstructLogo()
        {
            MSLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/M.png");
            MSLogoStyle = new GUIStyle();
            MSLogoStyle.margin = SuperHD ? new RectOffset(25, 0, 27, 33) : new RectOffset(15,0,15,15);
        }

        void ConstructField()
        {
            MSField = new GUIStyle();
            MSField.normal.textColor = new Color(1.0f, 1.0f, 1.0f);
            MSField.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/Field_Background.png");
            MSField.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Quixel/Scripts/Fonts/SourceSansPro-Regular.ttf");
            MSField.clipping = TextClipping.Clip;
            MSField.fontSize = SuperHD ? 24 : 13;
            MSField.padding = SuperHD ? new RectOffset(20, 0, 10, 0) : new RectOffset(10, 5, 7, 4);
            MSField.margin = SuperHD ? new RectOffset(0, 20, 13, 7) : new RectOffset(0, 10, 6, 5);
        }

        void ConstructCheckBox()
        {
            MSCheckBox = new GUIStyle();
            MSCheckBox.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/CheckBoxOff.png");
            MSCheckBox.hover.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/CheckBoxHover.png");
            MSCheckBox.onNormal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/CheckBoxOn.png");
        }

        void ConstructHelp()
        {
            MSHelpStyle = new GUIStyle();
            MSHelpStyle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Quixel/Scripts/Images/Help.png");
            MSHelpStyle.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Quixel/Scripts/Fonts/SourceSansPro-Regular.ttf");
            MSHelpStyle.margin = SuperHD ? new RectOffset(152, 20, 35, 15) : new RectOffset(102, 0, 16, 5);
            MSHelpStyle.padding = SuperHD ? new RectOffset(20, 20, 10, 10) : new RectOffset(10, 10, 5, 5);
            MSHelpStyle.fontSize = SuperHD ? 24 : 12;
            MSHelpStyle.normal.textColor = new Color(0.16796875f,0.59375f,0.9375f);
        }

        void ConstructConnection()
        {
            MSConnectStyle = new GUIStyle();
            MSConnectStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f);
            MSConnectStyle.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Quixel/Scripts/Fonts/SourceSansPro-Regular.ttf");
            MSConnectStyle.fontSize = SuperHD ? 24 : 13;
            MSConnectStyle.padding = SuperHD ? new RectOffset(5, 0, 15, 15) : new RectOffset(5, 5, 7, 4);
            MSConnectStyle.margin = SuperHD ? new RectOffset(72, 0, 27, 10) : new RectOffset(37, 20, 13, 5);
        }

        void InitStyle()
        {
            ConstructBackground();
            ConstructLogo();
            ConstructPopUp();
            ConstructText();
            ConstructField();
            ConstructCheckBox();
            ConstructHelp();
            ConstructConnection();
        }

        public static string GetPath()
        {
            return path;
        }

        public static int GetDispType()
        {
            return dispType;
        }

        private void Update()
        {
            if ((dispType != dispTypeUpdate) ||
                (shaderType != shaderTypeUpdate) ||
                (texPack != texPackUpdate) ||
                (path != pathUpdate) ||
                (prefix != prefixUpdate) ||
                (connection != connectionUpdate))
            {
                SaveDefaults();
            }
        }
    }
}

#endif