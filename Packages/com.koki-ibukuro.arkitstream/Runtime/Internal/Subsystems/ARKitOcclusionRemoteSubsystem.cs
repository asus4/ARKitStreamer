using System.Collections.Generic;
using System.Linq;
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
        static readonly int ARKIT_HUMAN_SEGMENTATION_ENABLED = Shader.PropertyToID("ARKIT_HUMAN_SEGMENTATION_ENABLED");

#if !UNITY_2020_2_OR_NEWER

        protected override Provider CreateProvider() => new ARKitProvider();
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
#if UNITY_EDITOR
            XROcclusionSubsystemCinfo info = new XROcclusionSubsystemCinfo()
            {
                id = ID,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(ARKitOcclusionRemoteSubsystem.ARKitProvider),
                subsystemTypeOverride = typeof(ARKitOcclusionRemoteSubsystem),
#else
                implementationType = typeof(ARKitOcclusionRemoteSubsystem),
#endif
                supportsHumanSegmentationStencilImage = true,
                supportsHumanSegmentationDepthImage = true,
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

