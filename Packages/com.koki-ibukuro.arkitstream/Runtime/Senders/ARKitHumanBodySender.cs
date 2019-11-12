using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;

namespace ARKitStream
{
    public class ARKitHumanBodySender : ARKitSubSender
    {
        [SerializeField] ARHumanBodyManager humanBodyManager = null;

        static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
        static readonly int _textureDepth = Shader.PropertyToID("_textureDepth");


        void OnValidate()
        {
            if (humanBodyManager == null)
            {
                humanBodyManager = FindObjectOfType<ARHumanBodyManager>();
            }
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {

        }

        protected override void OnNdiTransformer(Material material)
        {
            if (humanBodyManager == null || humanBodyManager.subsystem == null)
            {
                return;
            }
            material.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            material.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);
        }

        public static ARKitHumanBodySender TryCreate(ARKitSender sender)
        {
            var manager = FindObjectOfType<ARHumanBodyManager>();
            if (manager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitHumanBodySender>();
            self.humanBodyManager = manager;
            return self;
        }
    }
}
