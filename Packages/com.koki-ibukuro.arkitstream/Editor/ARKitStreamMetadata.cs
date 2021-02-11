using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace ARKitStream
{
    class ARKitStreamPackage : IXRPackage
    {

        class ARKitStreamLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName { get; set; }
            public string loaderType { get; set; }
            public List<BuildTargetGroup> supportedBuildTargets { get; set; }
        }

        class ARKitStreamPackageMetadata : IXRPackageMetadata
        {
            public string packageName { get; set; }
            public string packageId { get; set; }
            public string settingsType { get; set; }
            public List<IXRLoaderMetadata> loaderMetadata { get; set; }
        }

        private static IXRPackageMetadata s_Metadata = new ARKitStreamPackageMetadata()
        {
            packageName = "ARKit Stream",
            // packageId = "com.github.asus4.arkitstream",
            packageId = "com.koki-ibukuro.arkitstream",
            settingsType = typeof(ARKitStreamSetting).FullName,
            loaderMetadata = new List<IXRLoaderMetadata>()
            {
                new ARKitStreamLoaderMetadata()
                {
                    loaderName = "ARKit Stream",
                    loaderType = typeof(ARKitStreamLoader).FullName,
                    supportedBuildTargets = new List<BuildTargetGroup>()
                    {
                        BuildTargetGroup.Standalone,
                    }
                }
            },
        };

        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            return true;
        }
    }
}
