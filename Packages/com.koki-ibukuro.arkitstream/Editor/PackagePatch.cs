using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MockARFoundation
{
    [InitializeOnLoad]
    public static class PackagePatch
    {
        private const string PACKAGE_PATH = "Packages/com.unity.xr.arfoundation/Runtime/AR/";
        private const string PATCHES_PATH = "Packages/com.koki-ibukuro.arkitstream/Tools/";

        static PackagePatch()
        {
            AssetDatabase.DisallowAutoRefresh();

            bool isApplied = false;
            isApplied |= ApplyPatch(
                Path.Combine(PACKAGE_PATH, "ARCameraBackground.cs"),
                Path.Combine(PATCHES_PATH, "ARCameraBackground.cs.patch")
            );
            isApplied |= ApplyPatch(
                Path.Combine(PACKAGE_PATH, "ARFace.cs"),
                Path.Combine(PATCHES_PATH, "ARFace.cs.patch")
            );

            AssetDatabase.AllowAutoRefresh();
            if (isApplied)
            {
                AssetDatabase.Refresh();
            }
        }

        private static bool ApplyPatch(string filePath, string patchPath)
        {
            string patchFullPath = Path.GetFullPath(patchPath);
            var (originalText, replacingText) = LoadPatch(patchFullPath);

            var sourceAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)) as TextAsset;
            Debug.Assert(sourceAsset != null);

            var source = sourceAsset.text;
            if (source.Contains(replacingText))
            {
                Debug.Log($"The patch already applyed: {filePath}");
                return false;
            }
            var newSource = source.Replace(originalText, replacingText);
            var newAsset = new TextAsset(newSource);
            // EditorUtility.CopySerialized(newAsset, sourceAsset);
            // AssetDatabase.CreateAsset(newAsset, filePath);
            File.WriteAllText(Path.GetFullPath(filePath), newAsset.text);
            Debug.Log($"A patch applyed : {filePath}");
            return true;
        }

        private static Tuple<string, string> LoadPatch(string filePath)
        {
            Debug.Assert(File.Exists(filePath), $"File: {filePath} not found");

            var lines = File.ReadAllLines(filePath)
                .SkipWhile(line => !line.StartsWith("@@"))
                .Skip(1);

            var originals = lines
                .Where(line => !line.StartsWith("+"))
                .Select(line => line.Substring(1));

            var replaceWith = lines
                .Where(line => !line.StartsWith("-"))
                .Select(line => line.Substring(1));

            const string separator = "\n";
            return new Tuple<string, string>(string.Join(separator, originals), string.Join(separator, replaceWith));
        }

    }


}