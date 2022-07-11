using System.Linq;
using ARKitStream.Internal;
using Klak.Ndi;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream
{
    [DisallowMultipleComponent]
    public sealed class ARKitReceiver : MonoBehaviour
    {
        [SerializeField] private NdiResources resources = null;

        private NdiReceiver ndiReceiver = null;
        private Vector2Int ndiSourceSize = Vector2Int.zero;

        private CommandBuffer commandBuffer;
        private Material bufferMaterial;


        private RenderTexture[] renderTextures;
        private Texture2D[] texture2Ds;

        private readonly object packetLock = new object();
        private ARKitRemotePacket packet;

        public Texture2D YTexture => texture2Ds == null ? null : texture2Ds[0];
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
                return default;
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
                    return default;
                }
            }
        }

        public static ARKitReceiver Instance { get; private set; } = null;

        private void Awake()
        {
            // It works only in Editor!
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            if (Instance != null)
            {
                Debug.LogError("ARKitStreamReceiver must be only one in the scene.");
            }
            Instance = this;
        }

        private void Start()
        {
            ndiReceiver = gameObject.AddComponent<NdiReceiver>();
            ndiReceiver.SetResources(resources);
            var ndiName = FindNdiName();
            if (!string.IsNullOrWhiteSpace(ndiName))
            {
                ndiReceiver.ndiName = ndiName;
            }

            commandBuffer = new CommandBuffer();
            commandBuffer.name = "ARKitStreamReceiver";

            bufferMaterial = new Material(Shader.Find("Unlit/ARKitStreamReceiver"));

            SetupPose();
        }

        private void OnDisable()
        {
            Instance = null;

            commandBuffer?.Dispose();

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
        }

        private void Update()
        {
            var rt = ndiReceiver.texture;
            if (rt == null)
            {
                var ndiName = FindNdiName();
                if (!string.IsNullOrWhiteSpace(ndiName) && ndiReceiver.ndiName != ndiName)
                {
                    ndiReceiver.ndiName = ndiName;
                }
                return;
            }
            if (ndiSourceSize.x != rt.width || ndiSourceSize.y != rt.height)
            {
                InitTexture(rt);
                ndiSourceSize = new Vector2Int(rt.width, rt.height);
            }

            // Decode Metadata
            packet = ARKitRemotePacket.DeserializeFromNdiMetadata(ndiReceiver.metadata);

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

        private void OnGUI()
        {
            if (YTexture == null || CbCrTexture == null)
            {
                return;
            }
            GUI.DrawTexture(new Rect(0, 0, 256, 256), YTexture);
            GUI.DrawTexture(new Rect(0, 256, 256, 256), CbCrTexture);
        }

        private void Release(RenderTexture tex)
        {
            if (tex == null)
            {
                return;
            }
            tex.Release();
            Destroy(tex);
        }

        private void Release(Texture tex)
        {
            if (tex == null)
            {
                return;
            }
            Destroy(tex);
        }

        private void InitTexture(Texture source)
        {
            int width = source.width;
            int height = source.height / 2;

            var renderTexFormat = new RenderTextureFormat[] {
                RenderTextureFormat.R8, // Camera Y
                RenderTextureFormat.RG16, // Camera CbCr
                RenderTextureFormat.R8, // Stencil
                RenderTextureFormat.RHalf, // Depth
            };
            var texFormat = new TextureFormat[] {
                TextureFormat.R8,
                TextureFormat.RG16,
                TextureFormat.R8,
                TextureFormat.RHalf,
            };

            renderTextures = new RenderTexture[renderTexFormat.Length];
            texture2Ds = new Texture2D[renderTexFormat.Length];

            for (int i = 0; i < renderTexFormat.Length; i++)
            {
                renderTextures[i] = new RenderTexture(width, height, 0, renderTexFormat[i]);
                texture2Ds[i] = new Texture2D(width, height, texFormat[i], 1, false);
            }
        }

        private void SetupPose()
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
                return;
            }

            Debug.LogWarning("Pose Provider didn't found");
        }

        private static string FindNdiName()
        {
            return NdiFinder.sourceNames.FirstOrDefault();
        }
    }
}
