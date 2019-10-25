using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.VFX;

namespace ARKitStream
{
    public class ARKitVFX : MonoBehaviour
    {
        [SerializeField] VisualEffect visualEffect = null;
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] RenderTexture positionTexture = null;
        [SerializeField] RenderTexture colorTexture = null;

        Material convertMat;
        CommandBuffer commandBuffer;

        static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
        static readonly int _texutreDepth = Shader.PropertyToID("_texutreDepth");
        static readonly int _UnityDisplayTransform = Shader.PropertyToID("_UnityDisplayTransform");

        void OnEnable()
        {
            convertMat = new Material(Shader.Find("Hidden/ARKitVFXConvert"));
            commandBuffer = new CommandBuffer();

            cameraManager.frameReceived += OnFrameReceived;

            visualEffect.Play();
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

            if (args.projectionMatrix.HasValue)
            {
                convertMat.SetMatrix(_UnityDisplayTransform, args.projectionMatrix.Value);
            }

            commandBuffer.Blit(null, colorTexture, convertMat, 0);
            commandBuffer.Blit(null, positionTexture, convertMat, 1);

            Graphics.ExecuteCommandBuffer(commandBuffer);

            if (args.projectionMatrix.HasValue)
            {
                // TODO calc matrix
                var planes = args.projectionMatrix.Value.decomposeProjection;


                var tex = args.textures[0];
                float aspect = (float)tex.width / tex.height;
                var scale = new Vector3(aspect, -1, 1) * 5f;
                var t = visualEffect.transform;
                t.localScale = scale;
                t.localRotation = Quaternion.Euler(0, 0, -90);
                t.localPosition = new Vector3(0, 0, 10); // TODO


            }

        }


    }
}
