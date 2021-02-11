using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.XR.Management;

using UnityEngine;

namespace ARKitStream
{
    [XRCustomLoaderUI("ARKitStream.ARKitStreamLoader", BuildTargetGroup.Standalone)]
    public class ARKitStreamLoaderUI : IXRCustomLoaderUI
    {

        struct Content
        {
            public static readonly GUIContent k_LoaderName = new GUIContent("ARKit Stream");
        }

        #region IXRCustomLoaderUI Implementation

        public bool IsLoaderEnabled { get; set; }

        public string[] IncompatibleLoaders => new string[] { };

        public float RequiredRenderHeight { get; private set; }

        public void SetRenderedLineHeight(float height)
        {
            RequiredRenderHeight = height;
        }

        public BuildTargetGroup ActiveBuildTargetGroup { get; set; }

        public void OnGUI(Rect rect)
        {
            var size = EditorStyles.toggle.CalcSize(Content.k_LoaderName);
            var labelRect = new Rect(rect);
            labelRect.width = size.x;
            labelRect.height = RequiredRenderHeight;
            IsLoaderEnabled = EditorGUI.ToggleLeft(labelRect, Content.k_LoaderName, IsLoaderEnabled);
        }

        #endregion // IXRCustomLoaderUI Implementation
    }
}
