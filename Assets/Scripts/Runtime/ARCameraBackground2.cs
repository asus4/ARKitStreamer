using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEngine.XR.ARFoundation
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(ARCameraManager))]
    public class ARCameraBackground2 : MonoBehaviour
    {
        const string k_CustomRenderPassName = "AR Background Pass (LegacyRP)";
        internal const string k_MainTexName = "_MainTex";
        static readonly int k_DisplayTransformId = Shader.PropertyToID("_UnityDisplayTransform");

        Camera m_Camera;
        ARCameraManager m_CameraManager;
        CommandBuffer m_CommandBuffer;


        [SerializeField, FormerlySerializedAs("m_Material")]
        Material m_CustomMaterial;


        CameraClearFlags? m_PreviousCameraClearFlags;

        bool m_BackgroundRenderingEnabled;

        protected new Camera camera { get => m_Camera; }

        protected ARCameraManager cameraManager { get => m_CameraManager; }

        public Material material => m_CustomMaterial;

        bool useLegacyRenderPipeline { get => GraphicsSettings.renderPipelineAsset == null; }

        void Awake()
        {
            m_Camera = GetComponent<Camera>();
            m_CameraManager = GetComponent<ARCameraManager>();
        }

        void OnEnable()
        {
            // Ensure that background rendering is disabled until the first camera frame is received.
            m_BackgroundRenderingEnabled = false;
            cameraManager.frameReceived += OnCameraFrameReceived;
        }

        void OnDisable()
        {
            cameraManager.frameReceived -= OnCameraFrameReceived;
            DisableBackgroundRendering();
        }

        /// <summary>
        /// Enable background rendering by disabling the camera's clear flags, and enabling the legacy RP background
        /// rendering if we are in legacy RP mode.
        /// </summary>
        void EnableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = true;

            DisableBackgroundClearFlags();

            if (useLegacyRenderPipeline )
            {
                Debug.Log("enable legacy render pipeline!!!");
                EnableLegacyRenderPipelineBackgroundRendering();
            }
        }

        /// <summary>
        /// Disable background rendering by disabling the legacy RP background rendering if we are in legacy RP mode
        /// and restoring the camera's clear flags.
        /// </summary>
        void DisableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = false;

            DisableLegacyRenderPipelineBackgroundRendering();

            RestoreBackgroundClearFlags();

            // We are no longer setting the projection matrix so tell the camera to resume its normal projection matrix
            // calculations.
            camera.ResetProjectionMatrix();
        }

        /// <summary>
        /// Set the camera's clear flags to do nothing while preserving the previous camera clear flags.
        /// </summary>
        void DisableBackgroundClearFlags()
        {
            m_PreviousCameraClearFlags = m_Camera.clearFlags;
            m_Camera.clearFlags = CameraClearFlags.Nothing;
        }

        /// <summary>
        /// Restore the previous camera's clear flags, if any.
        /// </summary>
        void RestoreBackgroundClearFlags()
        {
            if (m_PreviousCameraClearFlags != null)
            {
                m_Camera.clearFlags = m_PreviousCameraClearFlags.Value;
            }
        }

        /// <summary>
        /// Enable background rendering getting a command buffer, and configure it for rendering the background.
        /// </summary>
        void EnableLegacyRenderPipelineBackgroundRendering()
        {
            if (m_CommandBuffer == null)
            {
                m_CommandBuffer = new CommandBuffer();
                m_CommandBuffer.name = k_CustomRenderPassName;

                ConfigureLegacyRenderPipelineBackgroundRendering(m_CommandBuffer);
            }
        }

        /// <summary>
        /// Disable background rendering by removing the command buffer from the camera.
        /// </summary>
        void DisableLegacyRenderPipelineBackgroundRendering()
        {
            if (m_CommandBuffer != null)
            {
                TeardownLegacyRenderPipelineBackgroundRendering(m_CommandBuffer);

                m_CommandBuffer = null;
            }
        }

        /// <summary>
        /// Configure the command buffer for background rendering by inserting the blit, and adding the command buffer
        /// into the camera.
        /// </summary>
        /// <param name="commandBuffer">The command buffer to configure.</param>
        protected virtual void ConfigureLegacyRenderPipelineBackgroundRendering(CommandBuffer commandBuffer)
        {
            Texture texture = !material.HasProperty(k_MainTexName) ? null : material.GetTexture(k_MainTexName);

            commandBuffer.ClearRenderTarget(true, false, Color.clear);
            commandBuffer.Blit(texture, BuiltinRenderTextureType.CameraTarget, material);
            camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
        }

        /// <summary>
        /// Teardown the command buffer that was configured for background rendering by removing the command buffer
        /// from the camera.
        /// </summary>
        /// <param name="commandBuffer">The command buffer to teaerdown.</param>
        protected virtual void TeardownLegacyRenderPipelineBackgroundRendering(CommandBuffer commandBuffer)
        {
            camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
        }

        /// <summary>
        /// Callback for the camera frame event.
        /// </summary>
        /// <param name="eventArgs">The camera event arguments.</param>
        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            Debug.Log("OnCameraFrameReceived");

            if (!m_BackgroundRenderingEnabled)
            {
                EnableBackgroundRendering();
            }

            Material material = this.material;
            if (material != null)
            {
                var count = eventArgs.textures.Count;
                for (int i = 0; i < count; ++i)
                {
                    material.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
                }

                if (eventArgs.displayMatrix.HasValue)
                {
                    material.SetMatrix(k_DisplayTransformId, eventArgs.displayMatrix.Value);
                }
            }

            if (eventArgs.projectionMatrix.HasValue)
            {
                camera.projectionMatrix = eventArgs.projectionMatrix.Value;
            }
        }
    }
}
