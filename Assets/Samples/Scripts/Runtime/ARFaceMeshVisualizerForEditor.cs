using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Generates a mesh for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// If this <c>GameObject</c> has a <c>MeshFilter</c> and/or <c>MeshCollider</c>,
    /// this component will generate a mesh from the underlying <c>XRFace</c>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.0/api/UnityEngine.XR.ARFoundation.ARFaceMeshVisualizer.html")]
    public sealed class ARFaceMeshVisualizerForEditor : MonoBehaviour
    {
        /// <summary>
        /// Get the <c>Mesh</c> that this visualizer creates and manages.
        /// </summary>
        public Mesh mesh { get; private set; }

        void SetVisible(bool visible)
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            if (m_MeshRenderer == null)
            {
                return;
            }

            //if it is getting visible after being invisible for a while, set its topology
            if (visible && !m_MeshRenderer.enabled)
            {
                SetMeshTopology();
            }

            m_MeshRenderer.enabled = visible;
        }

        static bool TryCopyToList<T>(NativeArray<T> array, List<T> list) where T : struct
        {
            list.Clear();
            if (!array.IsCreated || array.Length == 0)
                return false;

            foreach (var item in array)
                list.Add(item);

            return true;
        }

        void SetMeshTopology()
        {
            if (mesh == null)
            {
                return;
            }

            mesh.Clear();

            if (m_Face.vertices.IsCreated && m_Face.vertices.Length > 0 &&
                m_Face.indices.IsCreated && m_Face.indices.Length > 0)
            {
                m_Face.vertices.CopyTo(s_Vertices);
                mesh.vertices = s_Vertices;

                m_Face.indices.CopyTo(s_Indices);
                mesh.triangles = s_Indices;

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }

            if (m_Face.uvs.IsCreated && m_Face.uvs.Length > 0)
            {
                m_Face.uvs.CopyTo(s_UVs);
                mesh.uv = s_UVs;
            }

            // It throws error on Editor.

            // if (TryCopyToList(m_Face.vertices, s_Vertices) && TryCopyToList(m_Face.indices, s_Indices))
            // {
            //     mesh.SetVertices(s_Vertices);
            //     mesh.SetTriangles(s_Indices, 0);
            //     mesh.RecalculateBounds();

            //     if (TryCopyToList(m_Face.normals, s_Vertices))
            //     {
            //         mesh.SetNormals(s_Vertices);
            //     }
            //     else
            //     {
            //         mesh.RecalculateNormals();
            //     }
            // }

            // if (TryCopyToList(m_Face.uvs, s_UVs))
            // {
            //     mesh.SetUVs(0, s_UVs);
            // }



            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh = mesh;
            }

            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = mesh;
            }

            m_TopologyUpdatedThisFrame = true;
        }

        void UpdateVisibility()
        {
            var visible = enabled &&
                (m_Face.trackingState != TrackingState.None) &&
                (ARSession.state > ARSessionState.Ready);

            SetVisible(visible);
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            UpdateVisibility();
            if (!m_TopologyUpdatedThisFrame)
            {
                SetMeshTopology();
            }
            m_TopologyUpdatedThisFrame = false;
        }

        void OnSessionStateChanged(ARSessionStateChangedEventArgs eventArgs)
        {
            UpdateVisibility();
        }

        void Awake()
        {
            mesh = new Mesh();
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_Face = GetComponent<ARFace>();
            s_Indices = new int[0];
            s_Vertices = new Vector3[0];
            s_UVs = new Vector2[0];
        }

        void OnEnable()
        {
            m_Face.updated += OnUpdated;
            ARSession.stateChanged += OnSessionStateChanged;
            UpdateVisibility();
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            ARSession.stateChanged -= OnSessionStateChanged;
        }

        ARFace m_Face;
        MeshRenderer m_MeshRenderer;
        bool m_TopologyUpdatedThisFrame;
        static Vector3[] s_Vertices;
        static Vector2[] s_UVs;
        static int[] s_Indices;
    }
}
