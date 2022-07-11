using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{


    [Preserve]
    public class ARKitOcclusionRemoteSubsystem : XROcclusionSubsystem
    {
        public const string ID = "ARKit-Occlusion-Remote";

        static readonly int _HumanStencil = Shader.PropertyToID("_HumanStencil");
        static readonly int _HumanDepth = Shader.PropertyToID("_HumanDepth");
        // static readonly int ARKIT_HUMAN_SEGMENTATION_ENABLED = Shader.PropertyToID("ARKIT_HUMAN_SEGMENTATION_ENABLED");

        private static Supported DoesSupport() => Supported.Supported;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
#if UNITY_EDITOR
            XROcclusionSubsystemCinfo info = new XROcclusionSubsystemCinfo()
            {
                id = ID,
                providerType = typeof(ARKitOcclusionRemoteSubsystem.ARKitProvider),
                subsystemTypeOverride = typeof(ARKitOcclusionRemoteSubsystem),
                humanSegmentationStencilImageSupportedDelegate = DoesSupport,
                humanSegmentationDepthImageSupportedDelegate = DoesSupport,
            };

            if (!XROcclusionSubsystem.Register(info))
            {
                Debug.LogErrorFormat("Cannot register the {0} subsystem", ID);
            }
            else
            {
                Debug.LogFormat("Registered the {0} subsystem", ID);
            }
#endif // UNITY_EDITOR
        }

        class ARKitProvider : XROcclusionSubsystem.Provider
        {
            public override void Start() { }
            public override void Stop() { }
            public override void Destroy() { }

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
                var receiver = ARKitReceiver.Instance;
                if (receiver == null)
                {
                    humanStencilDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                var tex = receiver.StencilTexture;
                if (tex == null)
                {
                    humanStencilDescriptor = default(XRTextureDescriptor);
                    return false;
                }

                humanStencilDescriptor = new TextureDescriptor(tex, _HumanStencil);
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

                humanDepthDescriptor = new TextureDescriptor(tex, _HumanDepth);
                return true;
            }

            public override NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor,
                                                                                          Allocator allocator)
            {
                var descriptors = new List<XRTextureDescriptor>();

                XRTextureDescriptor descriptor;
                if (TryGetHumanStencil(out descriptor))
                {
                    descriptors.Add(descriptor);
                }
                if (TryGetHumanDepth(out descriptor))
                {
                    descriptors.Add(descriptor);
                }
                return new NativeArray<XRTextureDescriptor>(descriptors.ToArray(), allocator);
            }
        }
    }
}

