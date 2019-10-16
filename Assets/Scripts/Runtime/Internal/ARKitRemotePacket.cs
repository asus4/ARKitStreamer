using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;

namespace ARKitStream.Internal
{
    [Serializable]
    public class ARKitRemotePacket
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

            public static int DataSize => sizeof(long) + Marshal.SizeOf(typeof(Matrix4x4)) * 2;
        }

        public CameraFrameEvent cameraFrame;

        static BinaryFormatter formatter = new BinaryFormatter();
        static MemoryStream stream = new MemoryStream();

        readonly static byte[] buffer = new byte[CameraFrameEvent.DataSize];
        public byte[] Serialize()
        {
            stream.Seek(0, SeekOrigin.Begin);
            formatter.Serialize(stream, this);
            return stream.ToArray();
        }

        public static ARKitRemotePacket Deserialize(byte[] data)
        {
            // Debug.AssertFormat(data.Length == buffer.Length, $"data length: {data.Length} is not collect size of {buffer.Length}");
            stream.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(stream) as ARKitRemotePacket;
        }
    }

}
