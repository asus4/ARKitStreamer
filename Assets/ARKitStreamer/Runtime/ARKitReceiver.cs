using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using Klak.Ndi;
using WebSocketSharp;
using ARKitStream.Internal;

namespace ARKitStream
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NdiReceiver))]
    public sealed class ARKitReceiver : MonoBehaviour
    {
        [SerializeField] string ipAddress = "";
        [SerializeField] uint port = 8888;
        [SerializeField] bool isDrawGUI = false;

        NdiReceiver ndiReceiver = null;
        Vector2Int ndiSourceSize = Vector2Int.zero;

        CommandBuffer commandBuffer;
        Material bufferMaterial;


        RenderTexture[] renderTextures;
        Texture2D[] texture2Ds;
        WebSocket client;

        object packetLock = new object();
        ARKitRemotePacket packet;

        public Texture2D YTextrue => texture2Ds == null ? null : texture2Ds[0];
        public Texture2D CbCrTexture => texture2Ds == null ? null : texture2Ds[1];
        public Texture2D StencilTexture => texture2Ds == null ? null : texture2Ds[2];
        public Texture2D DepthTexture => texture2Ds == null ? null : texture2Ds[3];

        public ARKitRemotePacket.CameraFrameEvent CameraFrame
        {
            get
            {
                lock (packetLock)
                {
                    if (packet != null)
                    {
                        return packet.cameraFrame;
                    }
                }
                return default(ARKitRemotePacket.CameraFrameEvent);
            }
        }

        public static ARKitReceiver Instance { get; private set; } = null;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("ARKitStreamReceiver must be only one in the scene.");
            }
            Instance = this;
        }

        void Start()
        {
            // It works only in Editor!
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            ndiReceiver = GetComponent<NdiReceiver>();

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamReceiver";

            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamReceiver"));

            // Start WebSocket
            string wsAddress = $"ws://{ipAddress}:{port}/arkit";
            client = new WebSocket(wsAddress);
            client.OnOpen += (sender, e) =>
            {
                Debug.Log($"WebSocket Open: {wsAddress}");
            };
            client.OnMessage += OnWebsocketMessage;
            client.ConnectAsync();
        }

        void OnDestroy()
        {
            if (commandBuffer != null)
            {
                commandBuffer.Dispose();
                commandBuffer = null;
            }
            if (renderTextures != null)
            {
                foreach (var rt in renderTextures)
                {
                    Release(rt);
                }
            }
            if (texture2Ds != null)
            {
                foreach (var tex in texture2Ds)
                {
                    Release(tex);
                }
            }
            if (client != null)
            {
                if (client.IsAlive)
                {
                    client.CloseAsync();
                }
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

            // Decode Textures
            commandBuffer.Clear();
            for (int i = 0; i < renderTextures.Length; i++)
            {
                commandBuffer.Blit(rt, renderTextures[i], bufferMaterial, i);
            }
            Graphics.ExecuteCommandBuffer(commandBuffer);

            // RenderTexture -> Texture2D
            for (int i = 0; i < renderTextures.Length; i++)
            {
                Graphics.CopyTexture(renderTextures[i], texture2Ds[i]);
            }
        }

        void OnGUI()
        {
            if (!isDrawGUI)
            {
                return;
            }
            if (ndiSourceSize == Vector2Int.zero)
            {
                // Wait for connect
                return;
            }
            var w = Screen.width / 2;
            var h = Screen.height / 2;

            GUI.DrawTexture(new Rect(0, 0, w, h), texture2Ds[0]);
            GUI.DrawTexture(new Rect(w, 0, w, h), texture2Ds[1]);
            GUI.DrawTexture(new Rect(0, h, w, h), texture2Ds[2]);
            GUI.DrawTexture(new Rect(w, h, w, h), texture2Ds[3]);
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

        void OnWebsocketMessage(object sender, MessageEventArgs e)
        {
            try
            {
                lock (packetLock)
                {
                    packet = ARKitRemotePacket.Deserialize(e.RawData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

        }
    }
}
