using System;
using UnityEngine;
using UnityEngine.XR.Management;

using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using System.Linq;

namespace ARKitStream
{

    [System.Serializable]
    [XRConfigurationData("ARKit Stream", "ARKitStream.ARKitStreamSetting")]
    public class ARKitStreamSetting : ScriptableObject
    {
#if !UNITY_EDITOR
        public static ARKitStreamSetting s_RuntimeInstance = null;
#endif

        public enum Requirement
        {
            Required,
            Optional,
        }

        [SerializeField] Requirement m_Requirement = Requirement.Required;

        void Awake()
        {
#if !UNITY_EDITOR
            s_RuntimeInstance = this;
#endif
        }

    }
}
