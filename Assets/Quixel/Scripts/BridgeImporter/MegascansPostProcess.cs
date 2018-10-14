#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Quixel
{
    public class MegascansPostProcess : AssetPostprocessor
    {

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // loop through imported files, see if it's a .qxl file.
            for (int i = 0; i < importedAssets.Length; ++i)
            {
                if (importedAssets[i].Contains("MegascansImporterWindow.cs"))
                {
                    MegascansImporterWindow.Init();
                }
            }
        }
    }
}
#endif