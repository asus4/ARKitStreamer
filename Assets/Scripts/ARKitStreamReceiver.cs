using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;

namespace ARKitStream
{
    [RequireComponent(typeof(NdiReceiver))]
    public class ARKitStreamReceiver : MonoBehaviour
    {
        [SerializeField] ARCameraManager cameraManager = null;
        [SerializeField] ARHumanBodyManager humanBodyManager = null;

        NdiReceiver ndiReceiver = null;
        Vector2Int ndiSourceSize = Vector2Int.zero;

        CommandBuffer commandBuffer;
        Material bufferMaterial;


        RenderTexture[] renderTextures;
        Texture2D[] texture2Ds;


        void Start()
        {
            ndiReceiver = GetComponent<NdiReceiver>();

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamReceiver";

            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamReceiver"));
        }

        void OnDestroy()
        {
            commandBuffer.Dispose();
            commandBuffer = null;

            foreach (var rt in renderTextures)
            {
                Release(rt);
            }
            foreach (var tex in texture2Ds)
            {
                Release(tex);
            }

        }

        void Update()
        {
            var rt = ndiReceiver.receivedTexture;
            if (rt == null)
            {
                return;
            }
            if (ndiSourceSize.x != rt.width || ndiSourceSize.y != rt.height)
            {
                InitTexture(rt);
                ndiSourceSize = new Vector2Int(rt.width, rt.height);
            }

            commandBuffer.Clear();
            for (int i = 0; i < renderTextures.Length; i++)
            {
                commandBuffer.Blit(rt, renderTextures[i], bufferMaterial, i);
            }

            Graphics.ExecuteCommandBuffer(commandBuffer);

            // Graphics.CopyTexture(cameraYRT, cameraYT);
            // Graphics.CopyTexture(cameraCbCrRT, cameraCbCrT);
        }

        void OnGUI()
        {
            if (ndiSourceSize == Vector2Int.zero)
            {
                // Wait for connect
                return;
            }
            var w = Screen.width / 2;
            var h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), renderTextures[0]);
            GUI.DrawTexture(new Rect(w, 0, w, h), renderTextures[1]);
            GUI.DrawTexture(new Rect(0, h, w, h), renderTextures[2]);
            GUI.DrawTexture(new Rect(w, h, w, h), renderTextures[3]);
        }

        void Release(RenderTexture tex)
        {
            if (tex == null)
            {
                return;
            }
            tex.Release();
            Destroy(tex);
        }

        void Release(Texture tex)
        {
            if (tex == null)
            {
                return;
            }
            Destroy(tex);
        }

        void InitTexture(Texture source)
        {
            int width = source.width;
            int height = source.height / 2;

            var rformat = new RenderTextureFormat[] {
                RenderTextureFormat.R8, // Camera Y
                RenderTextureFormat.RG16, // Camera CbCr
                RenderTextureFormat.R8, // Stencil
                RenderTextureFormat.RHalf, // Depth
            };
            var tformat = new TextureFormat[] {
                TextureFormat.R8,
                TextureFormat.RG16,
                TextureFormat.R8,
                TextureFormat.RHalf,
            };

            renderTextures = new RenderTexture[rformat.Length];
            texture2Ds = new Texture2D[rformat.Length];

            for (int i = 0; i < rformat.Length; i++)
            {
                renderTextures[i] = new RenderTexture(width, height, 0, rformat[i]);
                texture2Ds[i] = new Texture2D(width, height, tformat[i], 1, false);
            }
        }

    }
}
