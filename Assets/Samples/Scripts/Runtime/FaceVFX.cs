using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARKitStream
{
    /// <summary>
    /// From 
    /// https://github.com/keijiro/Smrvfx
    /// </summary>
    public class FaceVFX : MonoBehaviour
    {
        [SerializeField] ARFaceManager faceManager = null;

        [SerializeField] RenderTexture positionTexture = null;
        [SerializeField] RenderTexture normalTexture = null;
        [SerializeField] ComputeShader compute = null;

        ComputeBuffer positionBuffer;
        ComputeBuffer normalBuffer;

        RenderTexture positionTextureRW;
        RenderTexture normalTextureRW;

        void OnEnable()
        {
            faceManager.facesChanged += OnFaceChanged;
        }

        void OnDisable()
        {
            faceManager.facesChanged -= OnFaceChanged;

            positionBuffer?.Dispose();
            normalBuffer?.Dispose();

            positionTextureRW?.Release();
            normalTextureRW?.Release();
        }

        void OnValidate()
        {
            if (faceManager == null)
            {
                faceManager = FindObjectOfType<ARFaceManager>();
            }
        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            if (args.updated == null | args.updated.Count == 0)
            {
                return;
            }

            var face = faceManager.TryGetFace(args.updated[0].trackableId);
            BakeTexture(face);
        }

        void BakeTexture(UnityEngine.XR.ARFoundation.ARFace face)
        {
            int length = face.vertices.Length;

            if (positionBuffer == null || positionBuffer.count != length * 3)
            {
                positionBuffer?.Dispose();
                normalBuffer?.Dispose();

                positionBuffer = new ComputeBuffer(length * 3, sizeof(float));
                normalBuffer = new ComputeBuffer(length * 3, sizeof(float));
            }

            if (positionTextureRW == null)
            {
                positionTextureRW = CreateRenderTextureRW(positionTexture);
                normalTextureRW = CreateRenderTextureRW(normalTexture);
            }


            compute.SetInt("VertexCount", length);
            compute.SetMatrix("Transform", face.transform.localToWorldMatrix);
            compute.SetFloat("FrameRate", 1 / Time.deltaTime);

            positionBuffer.SetData(face.vertices);
            normalBuffer.SetData(face.GetComponent<MeshFilter>().sharedMesh.normals);

            compute.SetBuffer(0, "PositionBuffer", positionBuffer);
            compute.SetBuffer(0, "NormalBuffer", normalBuffer);

            compute.SetTexture(0, "PositionMap", positionTextureRW);
            compute.SetTexture(0, "NormalMap", normalTextureRW);

            compute.Dispatch(0, positionTexture.width / 8, positionTexture.height / 8, 1);

            Graphics.CopyTexture(positionTextureRW, positionTexture);
            Graphics.CopyTexture(normalTextureRW, normalTexture);

        }

        static RenderTexture CreateRenderTextureRW(RenderTexture source)
        {
            var rt = new RenderTexture(source.width, source.height, 0, source.format);
            rt.enableRandomWrite = true;
            rt.Create();
            return rt;
        }

    }
}
