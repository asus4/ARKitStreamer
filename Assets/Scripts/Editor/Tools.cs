using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Klak.Ndi;

namespace ARKitStream
{
    public static class Tools
    {
        [MenuItem("Tools/Check Available NDI")]
        public static void CheckAvailableNDISource()
        {
            var names = NdiManager.GetSourceNames();
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("NDI Sources:");
            foreach (var name in names)
            {
                sb.AppendLine("- " + name);
            }
            Debug.Log(sb);
        }
    }
}