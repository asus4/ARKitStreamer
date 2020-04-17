using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    public partial class ARKitRemotePacket
    {
        [Serializable]
        public struct CameraFrameEvent : IEquatable<CameraFrameEvent>
        {
            // public ARLightEstimationData lightEstimationl;
            public long timestampNs;
            public float4x4 projectionMatrix;
            public float4x4 displayMatrix;
            // public double? exposureDuration;
            // public float? exposureOffset;

            public bool Equals(CameraFrameEvent o)
            {
                return timestampNs.Equals(o.timestampNs)
                    && projectionMatrix.Equals(o.projectionMatrix)
                    && displayMatrix.Equals(o.displayMatrix);
            }

            public override string ToString()
            {
                return $"[time: {timestampNs}, projection: {projectionMatrix}, display: {displayMatrix}]";
            }
        }

        [Serializable]
        public class FaceMesh
        {
            public TrackableId id;
            public byte[] vertices; // NativeArray<Vector3>
            public byte[] normals; // NativeArray<Vector3>
            public byte[] indices; // NativeArray<int>
            public byte[] uvs; // NativeArray<Vector2>
            public byte[] coefficients;

            public override string ToString()
            {
                return $"Mesh {id} verts: {vertices.Length} norms: {normals.Length} indices: {indices.Length} uvs: {uvs.Length}";
            }
        }

        [Serializable]
        public class FaceInfo
        {
            public XRFace[] added;
            public XRFace[] updated;
            public TrackableId[] removed;
            public FaceMesh[] meshes;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("FaceInfo");
                foreach (var f in added)
                {
                    sb.AppendLine($"ADD: {f}");
                }
                foreach (var f in updated)
                {
                    sb.AppendLine($"UPDATE: {f}");
                }
                foreach (var f in removed)
                {
                    sb.AppendLine($"REMOVE: {f}");
                }
                foreach (var m in meshes)
                {
                    sb.AppendLine($"MESHED: {m}");
                }
                return sb.ToString();
            }
        }

        [Serializable]
        public class PlaneInfo
        {
            public BoundedPlane[] added;
            public BoundedPlane[] updated;
            public TrackableId[] removed;
            public Dictionary<TrackableId, byte[]> meshes;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("PlaneInfo");
                foreach (var f in added)
                {
                    sb.AppendLine($"ADD: {f}");
                }
                foreach (var f in updated)
                {
                    sb.AppendLine($"UPDATE: {f}");
                }
                foreach (var f in removed)
                {
                    sb.AppendLine($"REMOVE: {f}");
                }
                foreach (var m in meshes)
                {
                    sb.AppendLine($"MESHED: {m}");
                }
                return sb.ToString();
            }
        }

        [Serializable]
        public class HumanBodyInfo
        {
            public XRHumanBody[] added;
            public XRHumanBody[] updated;
            public TrackableId[] removed;
            public Dictionary<TrackableId, byte[]> joints;

            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("HumanBodyInfo:");
                foreach (var f in added)
                {
                    sb.AppendLine($"ADD: {f}");
                }
                foreach (var f in updated)
                {
                    sb.AppendLine($"UPDATE: {f}");
                }
                foreach (var f in removed)
                {
                    sb.AppendLine($"REMOVE: {f}");
                }
                return sb.ToString();
            }
        }
    }

}
