using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;
using Klak.Ndi;
using WebSocketSharp;
using ARKitStream.Internal;

namespace ARKitStream
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NdiReceiver))]
    public sealed class ARKitReceiver : MonoBehaviour
    {
        [SerializeField] string ipAddress = "172.20.10.1";
        [SerializeField] uint port = 8888;

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

        public TrackingState trackingState
        {
            get
            {
                lock (packetLock)
                {
                    if (texture2Ds != null && packet != null)
                    {
                        return TrackingState.Tracking;
                    }
                    return TrackingState.None;
                }
            }
        }

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

        public ARKitRemotePacket.FaceInfo Face
        {
            get
            {
                lock (packetLock)
                {
                    if (packet != null)
                    {
                        return packet.face;
                    }
                    return null;
                }
            }
        }

        public ARKitRemotePacket.PlaneInfo Plane
        {
            get
            {
                lock (packetLock)
                {
                    if (packet != null)
                    {
                        return packet.plane;
                    }
                    return null;
                }
            }
        }

        public ARKitRemotePacket.HumanBodyInfo HumanBody
        {
            get
            {
                lock (packetLock)
                {
                    if (packet != null)
                    {
                        return packet.humanBody;
                    }
                    return null;
                }
            }
        }

        public UnityEngine.Pose TrackedPose
        {
            get
            {
                lock (packetLock)
                {
                    if (packet != null)
                    {
                        return packet.trackedPose;
                    }
                    return default(UnityEngine.Pose);
                }
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

            SetupPose();
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

        void SetupPose()
        {
            var trackedPoseDriver = FindObjectOfType<UnityEngine.SpatialTracking.TrackedPoseDriver>();
            if (trackedPoseDriver != null)
            {
                var provider = gameObject.GetComponent<ARKitRemotePoseProvider>()
                            ?? gameObject.AddComponent<ARKitRemotePoseProvider>();
                trackedPoseDriver.poseProviderComponent = provider;
                return;
            }

            var arPoseDriver = FindObjectOfType<UnityEngine.XR.ARFoundation.ARPoseDriver>();
            if (arPoseDriver != null)
            {
                var provider = gameObject.GetComponent<ARKitRemotePoseProvider>()
                            ?? gameObject.AddComponent<ARKitRemotePoseProvider>();
                provider.manualTarget = arPoseDriver.transform;
            }
        }
    }
}
