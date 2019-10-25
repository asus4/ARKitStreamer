using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.VFX;

namespace ARKitStream
{
    [RequireComponent(typeof(VisualEffect))]
    public class ARKitVFX : MonoBehaviour
    {
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] RenderTexture positionTexture = null;
        [SerializeField] RenderTexture colorTexture = null;

        Material convertMat;
        CommandBuffer commandBuffer;

        static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
        static readonly int _texutreDepth = Shader.PropertyToID("_texutreDepth");

        void OnEnable()
        {
            convertMat = new Material(Shader.Find("Hidden/ARKitVFXConvert"));
            commandBuffer = new CommandBuffer();

            cameraManager.frameReceived += OnFrameReceived;


            var effect = GetComponent<VisualEffect>();
            effect.Play();
        }

        void OnDisable()
        {
            if (convertMat != null)
            {
                Destroy(convertMat);
            }
            commandBuffer?.Dispose();
            commandBuffer = null;

            cameraManager.frameReceived -= OnFrameReceived;
        }

        void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            commandBuffer.Clear();

            for (int i = 0; i < args.textures.Count; i++)
            {
                convertMat.SetTexture(args.propertyNameIds[i], args.textures[i]);
            }

            convertMat.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            convertMat.SetTexture(_texutreDepth, humanBodyManager.humanDepthTexture);

            commandBuffer.Blit(null, colorTexture, convertMat, 0);
            commandBuffer.Blit(null, positionTexture, convertMat, 1);

            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

    }
}
