using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace ARKitStream
{
    [InitializeOnLoad]
    public static class PackagePatch
    {
        static PackagePatch()
        {
            string packagePath = Path.GetFullPath("Packages/com.unity.xr.arfoundation/Runtime/AR/");
            string toolsPath = Path.GetFullPath("Packages/com.koki-ibukuro.arkitstream/Tools/");
            string scriptPath = Path.GetFullPath("Packages/com.koki-ibukuro.arkitstream/Tools/apply_patch.sh");

            RunCommand(
                scriptPath,
                Path.Combine(packagePath, "ARCameraBackground.cs"),
                Path.Combine(toolsPath, "ARCameraBackground.cs.patch")
            );

            RunCommand(
                scriptPath,
                Path.Combine(packagePath, "ARFace.cs"),
                Path.Combine(toolsPath, "ARFace.cs.patch")
            );
        }

        private static bool RunCommand(string scriptPath, string filePath, string patchPath)
        {
            Debug.Assert(File.Exists(scriptPath), $"File: {scriptPath} not found");
            Debug.Assert(File.Exists(filePath), $"File: {filePath} not found");
            Debug.Assert(File.Exists(patchPath), $"File: {patchPath} not found");

            var info = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = $"{scriptPath} {filePath} {patchPath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var process = Process.Start(info);
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // if (!string.IsNullOrWhiteSpace(stdout))
            // {
            //     Debug.Log($"stdout: {stdout}");
            // }
            // if (!string.IsNullOrWhiteSpace(stderr))
            // {
            //     Debug.LogError($"stderr: {stderr}");
            // }

            return true;
        }

    }


}
