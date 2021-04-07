using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ARKitStream
{
    public static class PackagePatch
    {
        private const string MENU = "Window/Tools/ARKit Stream/";
        private const string AR_FOUNDATION_PACKAGE = "Packages/com.unity.xr.arfoundation/";
        private const string PATCHES_PATH = "Packages/com.koki-ibukuro.arkitstream/Tools/";

        [MenuItem(MENU + "Apply Patches")]
        private static void ApplyPatches()
        {
            AssetDatabase.DisallowAutoRefresh();

            string globalCachePath = GetPackageCachePath(AR_FOUNDATION_PACKAGE);
            ApplyPatch(
                Path.Combine(PATCHES_PATH, "ARCameraBackground.cs.patch"),
                Path.Combine(AR_FOUNDATION_PACKAGE, "Runtime/AR/ARCameraBackground.cs"),
                Path.Combine(globalCachePath, "Runtime/AR/ARCameraBackground.cs")
            );
            ApplyPatch(
                Path.Combine(PATCHES_PATH, "ARFace.cs.patch"),
                Path.Combine(AR_FOUNDATION_PACKAGE, "Runtime/AR/ARFace.cs"),
                Path.Combine(globalCachePath, "Runtime/AR/ARFace.cs")
            );

            AssetDatabase.AllowAutoRefresh();
        }

        [MenuItem(MENU + "Revert Patches")]
        private static void RevertPatches()
        {
            var originalCache = GetPackageCachePath(AR_FOUNDATION_PACKAGE);
            DeleteDirectory(originalCache);

            var projectCache = Path.GetFullPath(AR_FOUNDATION_PACKAGE);
            DeleteDirectory(projectCache);
        }

        private static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static string GetPackageCachePath(string packageName)
        {
            if (Application.platform != RuntimePlatform.OSXEditor)
            {
                throw new System.NotImplementedException($"Not implemented for platform: {Application.platform}");
            }
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(packageName);
            if (info == null)
            {
                throw new System.ArgumentException($"Package: {packageName} did not find");
            }

            // TODO: this only works with macOS. Confirm other platforms.
            // https://docs.unity3d.com/Manual/upm-cache.html
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, "Library/Unity/cache/packages/packages.unity.com", info.packageId);
        }

        private static bool ApplyPatch(string patchPath, string localCachePath, string globalCachePath)
        {
            var (originalText, replacingText) = LoadPatch(Path.GetFullPath(patchPath));

            var sourceAsset = AssetDatabase.LoadAssetAtPath(localCachePath, typeof(TextAsset)) as TextAsset;
            Debug.Assert(sourceAsset != null);

            var source = sourceAsset.text;
            if (source.Contains(replacingText))
            {
                Debug.Log($"The patch already applyed: {localCachePath}");
                return false;
            }
            var newSource = source.Replace(originalText, replacingText);
            var newAsset = new TextAsset(newSource);
            File.WriteAllText(Path.GetFullPath(globalCachePath), newAsset.text);
            File.WriteAllText(Path.GetFullPath(localCachePath), newAsset.text);
            Debug.Log($"A patch applyed : {localCachePath}");
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
