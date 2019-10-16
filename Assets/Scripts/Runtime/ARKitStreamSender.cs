using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;

namespace ARKitStream
{
    public class ARKitStreamSender : MonoBehaviour
    {
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;
        [SerializeField] Material previewMaterial = null;
        [SerializeField] RawImage debugImage = null;

        static readonly int _textureStencil = Shader.PropertyToID("_textureStencil");
        static readonly int _textureDepth = Shader.PropertyToID("_textureDepth");

        static readonly int k_DisplayTransformId = Shader.PropertyToID("_UnityDisplayTransform");

        Material bufferMaterial;
        RenderTexture renderTexture;
        NdiSender ndiSender;
        CommandBuffer commandBuffer;

        void Start()
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamSender";
            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamSender"));
            cameraManager.frameReceived += OnCameraFarameReceived;
        }

        void OnDestroy()
        {
            cameraManager.frameReceived -= OnCameraFarameReceived;

            Destroy(bufferMaterial);
            bufferMaterial = null;
        }

        void OnCameraFarameReceived(ARCameraFrameEventArgs args)
        {
            var subsystem = humanBodyManager.subsystem;
            if (subsystem == null)
            {
                // ARKit is not supported on this device
                return;
            }

            ShowTextureInfo(ref args);

            previewMaterial.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            previewMaterial.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);


            if (renderTexture == null)
            {
                int width = args.textures.Max(t => t.width);
                int height = args.textures.Max(t => t.height);
                InitNDI(width, height);
            }

            // Set texture
            var count = args.textures.Count;
            for (int i = 0; i < count; i++)
            {
                bufferMaterial.SetTexture(args.propertyNameIds[i], args.textures[i]);
            }
            bufferMaterial.SetTexture(_textureStencil, humanBodyManager.humanStencilTexture);
            bufferMaterial.SetTexture(_textureDepth, humanBodyManager.humanDepthTexture);

            commandBuffer.Blit(null, renderTexture, bufferMaterial);
            Graphics.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            //Graphics.Blit(null, renderTexture, bufferMaterial);
        }

        void ShowTextureInfo(ref ARCameraFrameEventArgs args)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var t in args.textures)
            {
                sb.AppendFormat("Texture {0}: ({1} , {2}) format: {3}\n", t.name, t.width, t.height, t.format);
            }
            var tex = humanBodyManager.humanStencilTexture;
            sb.AppendFormat("Stencil {0}: ({1} , {2}) format: {3}\n", tex.name, tex.width, tex.height, tex.format);
            tex = humanBodyManager.humanDepthTexture;
            sb.AppendFormat("Depth {0}: ({1} , {2}) format: {3}\n", tex.name, tex.width, tex.height, tex.format);

            Debug.Log(sb);
        }

        void InitNDI(int width, int height)
        {
            renderTexture = new RenderTexture(width, height * 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            var name = string.Format("ARKit Stream {0:0000}", Random.Range(100, 9999));
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            ndiSender = go.AddComponent<NdiSender>();

            ndiSender.sourceTexture = renderTexture;
            ndiSender.alphaSupport = false;

            debugImage.texture = renderTexture;
        }

    }
}
