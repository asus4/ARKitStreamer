using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;

namespace ARKitStream
{
    public class ARKitHumanBodySender : ARKitSubSender
    {
        [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] Material previewMaterial = null;

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

            previewMaterial.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            previewMaterial.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);

            material.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            material.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);
        }
    }
}