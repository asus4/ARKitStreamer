using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;

namespace ARKitStream
{
    public class ARKitHumanBodySender : ARKitSubSender
    {
        // [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] AROcclusionManager occlusionManager = null;

        static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
        static readonly int _textureDepth = Shader.PropertyToID("_textureDepth");


        void OnValidate()
        {
            if (occlusionManager == null)
            {
                occlusionManager = FindObjectOfType<AROcclusionManager>();
            }
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {

        }

        protected override void OnNdiTransformer(Material material)
        {
            if (occlusionManager == null || occlusionManager.subsystem == null)
            {
                return;
            }
            material.SetTexture(_textureStencil, occlusionManager.humanStencilTexture);
            material.SetTexture(_textureDepth, occlusionManager.humanDepthTexture);
        }

        public static ARKitHumanBodySender TryCreate(ARKitSender sender)
        {
            var manager = FindObjectOfType<AROcclusionManager>();
            if (manager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitHumanBodySender>();
            self.occlusionManager = manager;
            return self;
        }
    }
}
