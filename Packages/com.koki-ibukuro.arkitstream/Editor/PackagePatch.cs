using System;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace MockARFoundation
{
    [InitializeOnLoad]
    public static class PackagePatch
    {
        static PackagePatch()
        {
            string packagePath = Path.GetFullPath("Packages/com.unity.xr.arfoundation/Runtime/AR/");
            string toolsPath = Path.GetFullPath("Packages/com.koki-ibukuro.arkitstream/Tools/");
            ApplyPatch(
                Path.Combine(packagePath, "ARCameraBackground.cs"),
                Path.Combine(toolsPath, "ARCameraBackground.cs.patch")
            );
            ApplyPatch(
                Path.Combine(packagePath, "ARFace.cs"),
                Path.Combine(toolsPath, "ARFace.cs.patch")
            );
        }

        private static bool ApplyPatch(string filePath, string patchPath)
        {
            Debug.Assert(File.Exists(filePath), $"File: {filePath} not found");
            Debug.Assert(File.Exists(patchPath), $"File: {patchPath} not found");

            var (originalText, replacingText) = LoadPatch(patchPath);
            // Debug.Log($"original:\n {originalText}");
            // Debug.Log($"replace with:\n {replacingText}");

            var source = File.ReadAllText(filePath);
            if (!source.Contains(originalText))
            {
                return false;
            }
            Debug.Log($"A patch applyed : {filePath}");
            var newSource = source.Replace(originalText, replacingText);
            // Debug.Assert(source != newSource);
            File.WriteAllText(filePath, newSource);
            return true;
        }

        private static Tuple<string, string> LoadPatch(string filePath)
        {
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