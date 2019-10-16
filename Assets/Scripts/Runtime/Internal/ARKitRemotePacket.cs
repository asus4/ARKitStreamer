using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;
using MessagePack;

namespace ARKitStream.Internal
{
    [MessagePackObject]
    public class ARKitRemotePacket
    {
        [MessagePackObject]
        public struct CameraFrameEvent
        {
            // public ARLightEstimationData lightEstimationl;
            [Key(0)]
            public long timestampNs;
            [Key(1)]
            public float4x4 projectionMatrix;
            [Key(2)]
            public float4x4 displayMatrix;
            // public double? exposureDuration;
            // public float? exposureOffset;
        }

        [Key(0)]
        public CameraFrameEvent cameraFrame;


        public byte[] ToData()
        {
            return MessagePackSerializer.Serialize<ARKitRemotePacket>(this);
        }

        public static ARKitRemotePacket FromData(byte[] data)
        {
            return MessagePackSerializer.Deserialize<ARKitRemotePacket>(data);
        }
    }

}
