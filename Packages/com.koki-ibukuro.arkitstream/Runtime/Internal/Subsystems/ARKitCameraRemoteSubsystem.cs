using System;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;
#elif MODULE_LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
#endif
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



namespace ARKitStream.Internal
{
    [Preserve]
    public sealed class ARKitCameraRemoteSubsystem : XRCameraSubsystem
    {
        public const string ID = "ARKit-Camera-Remote";
        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
#if UNITY_EDITOR
            XRCameraSubsystemCinfo cameraSubsystemCinfo = new XRCameraSubsystemCinfo
            {
                id = ID,
                implementationType = typeof(ARKitCameraRemoteSubsystem),
                supportsAverageBrightness = false,
                supportsAverageColorTemperature = true,
                supportsColorCorrection = false,
                supportsDisplayMatrix = true,
                supportsProjectionMatrix = true,
                supportsTimestamp = true,
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
                supportsAverageIntensityInLumens = true,
                supportsFocusModes = true,
                supportsFaceTrackingAmbientIntensityLightEstimation = true,
                supportsFaceTrackingHDRLightEstimation = true,
                supportsWorldTrackingAmbientIntensityLightEstimation = true,
                supportsWorldTrackingHDRLightEstimation = false,
            };

            if (!XRCameraSubsystem.Register(cameraSubsystemCinfo))
            {
                Debug.LogErrorFormat("Cannot register the {0} subsystem", ID);
            }
            else
            {
                Debug.LogFormat("Registered the {0} subsystem", ID);
            }
#endif // UNITY_EDITOR
        }

        class ARKitRemoteProvider : Provider
        {
            static readonly int _TEXTURE_Y = Shader.PropertyToID("_textureY");
            static readonly int _TEXTURE_CB_CR = Shader.PropertyToID("_textureCbCr");

            Material m_CameraMaterial;
            public override Material cameraMaterial => m_CameraMaterial;

            public override bool permissionGranted => true;

            private static string ShaderName
            {
                get
                {
                    var pipeline = GraphicsSettings.renderPipelineAsset;
                    if (pipeline == null)
                    {
                        return "Unlit/ARKitBackground";
                    }
#if MODULE_URP_ENABLED
                    else if (pipeline is UniversalRenderPipelineAsset)
                    {
                        return "Unlit/ARKitURPBackground";
                    }
#elif MODULE_LWRP_ENABLED
                    else if (pipeline is LightweightRenderPipelineAsset)
                    {
                        return "Unlit/ARKitLWRPBackground";
                    }
#endif
                    Debug.LogError($"{pipeline} is not supported in ARKit");
                    return "Unlit/ARKitBackground";
                }
            }

            public ARKitRemoteProvider()
            {
                m_CameraMaterial = CreateCameraMaterial(ShaderName);
            }

            public override Feature currentCamera => Feature.AnyCamera;

            public override Feature requestedCamera
            {
                get => Feature.AnyCamera;
                set => Debug.Log($"requestedCamera: {value}");
            }

            public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
            {
                var remote = ARKitReceiver.Instance;
                if (remote == null)
                {
                    cameraFrame = default(XRCameraFrame);
                    return false;
                }


                var remoteFrame = ARKitReceiver.Instance.CameraFrame;
                if (remoteFrame.timestampNs == default(long))
                {
                    cameraFrame = default(XRCameraFrame);
                    return false;
                }

                const XRCameraFrameProperties properties =
                    XRCameraFrameProperties.Timestamp
                    | XRCameraFrameProperties.ProjectionMatrix
                    | XRCameraFrameProperties.DisplayMatrix;

                cameraFrame = new CameraFrame()
                {
                    timestampNs = remoteFrame.timestampNs,
                    averageBrightness = 0,
                    averageColorTemperature = 0,
                    colorCorrection = default(Color),
                    projectionMatrix = remoteFrame.projectionMatrix,
                    displayMatrix = remoteFrame.displayMatrix,
                    trackingState = TrackingState.Tracking,
                    nativePtr = new IntPtr(0),
                    properties = properties,
                    averageIntensityInLumens = 0,
                    exposureDuration = 0,
                    exposureOffset = 0
                };

                // Debug.Log(cameraFrame);
                return true;
            }

            public override bool autoFocusEnabled => true;

            public override bool autoFocusRequested
            {
                get => true;
                set => Debug.Log($"autoFocusRequested: {value}");
            }

            public override Feature currentLightEstimation => Feature.AnyLightEstimation;
            public override Feature requestedLightEstimation
            {
                get => Feature.AnyLightEstimation;
                set => Debug.Log($"requestedLightEstimation: {value}");
            }





            public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor, Allocator allocator)
            {
                var remote = ARKitReceiver.Instance;
                if (remote == null)
                {
                    return new NativeArray<XRTextureDescriptor>(0, allocator);
                }

                var yTex = remote.YTextrue;
                var cbcrTex = remote.CbCrTexture;
                if (yTex == null || cbcrTex == null)
                {
                    return new NativeArray<XRTextureDescriptor>(0, allocator);
                }


                var arr = new NativeArray<XRTextureDescriptor>(2, allocator);
                arr[0] = new TextureDescriptor(yTex, _TEXTURE_Y);
                arr[1] = new TextureDescriptor(cbcrTex, _TEXTURE_CB_CR);

                return arr;
            }

        }
    }
}
