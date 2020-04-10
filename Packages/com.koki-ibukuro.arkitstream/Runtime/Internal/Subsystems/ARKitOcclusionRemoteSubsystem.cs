using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    internal static class ARKitOcclusionRemoteRegistration
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
#if UNITY_EDITOR
            const string k_SubsystemId = "ARKit-Occlusion-Remote";
            XROcclusionSubsystemCinfo info = new XROcclusionSubsystemCinfo()
            {
                id = k_SubsystemId,
                implementationType = typeof(ARKitOcclusionRemoteSubsystem),
                supportsHumanSegmentationStencilImage = true,
                supportsHumanSegmentationDepthImage = true,
            };

            if (!XROcclusionSubsystem.Register(info))
            {
                Debug.LogErrorFormat("Cannot register the {0} subsystem", k_SubsystemId);
            }
            else
            {
                Debug.LogFormat("Registered the {0} subsystem", k_SubsystemId);
            }
#endif // UNITY_EDITOR
        }
    }


    [Preserve]
    public class ARKitOcclusionRemoteSubsystem : XROcclusionSubsystem
    {
        protected override Provider CreateProvider() => new ARKitProvider();

        class ARKitProvider : XROcclusionSubsystem.Provider
        {
            public override HumanSegmentationStencilMode requestedHumanStencilMode
            {
                get => HumanSegmentationStencilMode.Best;
                set { }
            }

            public override HumanSegmentationStencilMode currentHumanStencilMode => HumanSegmentationStencilMode.Best;

            public override HumanSegmentationDepthMode requestedHumanDepthMode
            {
                get => HumanSegmentationDepthMode.Best;
                set { }
            }

            public override HumanSegmentationDepthMode currentHumanDepthMode => HumanSegmentationDepthMode.Best;

            public override bool TryGetHumanStencil(out XRTextureDescriptor humanStencilDescriptor)
            {
                var recevier = ARKitReceiver.Instance;
                if (recevier == null)
                {
                    humanStencilDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                var tex = recevier.StencilTexture;
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

