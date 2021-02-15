using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace ARKitStream
{
    [InitializeOnLoad]
    public class PackagePatch
    {
        static PackagePatch()
        {

            var info = new ProcessStartInfo()
            {
                FileName = "/bin/sh",
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = "Tools/apply_patch.sh",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var process = Process.Start(info);
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Debug.Log("Applied patches");

            // if (!string.IsNullOrWhiteSpace(stdout))
            // {
            //     Debug.Log($"stdout: {stdout}");
            // }
            // if (!string.IsNullOrWhiteSpace(stderr))
            // {
            //     Debug.LogError($"stderr: {stderr}");
            // }
        }
    }
}
