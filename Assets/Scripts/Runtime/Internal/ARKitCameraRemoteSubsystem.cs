using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [Preserve]
    public sealed class ARKitCameraRemoteSubsystem : XRCameraSubsystem
    {
        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            const string id = "ARKit-Camera-Remote";
#if UNITY_EDITOR
            XRCameraSubsystemCinfo cameraSubsystemCinfo = new XRCameraSubsystemCinfo
            {
                id = id,
                implementationType = typeof(ARKitCameraRemoteSubsystem),
                supportsAverageBrightness = false,
                supportsAverageColorTemperature = true,
                supportsColorCorrection = false,
                supportsDisplayMatrix = true,
                supportsProjectionMatrix = true,
                supportsTimestamp = true,
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
                supportsAverageIntensityInLumens = true
            };

            if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
            {
                Debug.LogErrorFormat("Cannot register the {0} subsystem", id);
            }
#endif // UNITY_EDITOR
        }

        class ARKitRemoteProvider : Provider
        {
            static readonly int k_TextureYPropertyNameId = Shader.PropertyToID("_textureY");
            static readonly int k_TextureCbCrPropertyNameId = Shader.PropertyToID("_textureCbCr");

            Material m_CameraMaterial;
            public override Material cameraMaterial => m_CameraMaterial;

            public override bool permissionGranted => true;

            public ARKitRemoteProvider()
            {
                m_CameraMaterial = CreateCameraMaterial("Unlit/ARKitBackground");
            }
        }
    }
}