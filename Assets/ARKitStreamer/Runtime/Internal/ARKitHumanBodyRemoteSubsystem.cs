using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [Preserve]
    public class ARKitHumanBodyRemoteSubsystem : XRHumanBodySubsystem
    {
        protected override Provider CreateProvider()
        {
            return new ARKitProvider();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
#if UNITY_EDITOR
            const string k_SubsystemId = "ARKit-HumanBody-Remote";
            XRHumanBodySubsystemCinfo info = new XRHumanBodySubsystemCinfo()
            {
                id = k_SubsystemId,
                implementationType = typeof(ARKitHumanBodyRemoteSubsystem),
                supportsHumanBody2D = false,
                supportsHumanBody3D = false,
                supportsHumanBody3DScaleEstimation = false,
                supportsHumanStencilImage = true,
                supportsHumanDepthImage = true,
            };

            if (!XRHumanBodySubsystem.Register(info))
            {
                Debug.LogErrorFormat("Cannot register the {0} subsystem", k_SubsystemId);
            }
            else
            {
                Debug.LogFormat("Registered the {0} subsystem", k_SubsystemId);
            }
#endif // UNITY_EDITOR
        }

        class ARKitProvider : XRHumanBodySubsystem.Provider
        {
            public override TrackableChanges<XRHumanBody> GetChanges(XRHumanBody defaultHumanBody, Unity.Collections.Allocator allocator)
            {
                return new TrackableChanges<XRHumanBody>();
            }

            public override bool TrySetHumanSegmentationStencilMode(HumanSegmentationMode humanSegmentationStencilMode)
            {
                return true;
            }

            public override bool TrySetHumanSegmentationDepthMode(HumanSegmentationMode humanSegmentationDepthMode)
            {
                return true;
            }

            public override bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
            {
                if (ARKitReceiver.Instance == null)
                {
                    humanStencilDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                var tex = ARKitReceiver.Instance.StencilTexture;
                if (tex == null)
                {
                    humanStencilDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                humanStencilDescriptor = new TextureDescriptor(tex, 0);
                return true;
            }

            public override bool TryGetHumanDepth(out XRTextureDescriptor humanDepthDescriptor)
            {
                var receiver = ARKitReceiver.Instance;
                if (receiver == null)
                {
                    humanDepthDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                var tex = receiver.DepthTexture;
                if (tex == null)
                {
                    humanDepthDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                humanDepthDescriptor = new TextureDescriptor(tex, 0);
                return true;
            }
        }
    }
}

