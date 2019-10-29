using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using ARKitStream.Internal;
using Pose = ARKitStream.Internal.Pose;
using XRFace = ARKitStream.Internal.XRFace;

namespace ARKitStream
{
    public class ARKitFaceSender : ARKitSubSender
    {
        [SerializeField] ARFaceManager faceManager = null;

        ARKitRemotePacket.FaceInfo info = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            faceManager.facesChanged += OnFaceChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            faceManager.facesChanged -= OnFaceChanged;
        }

        void OnValidate()
        {
            if (faceManager == null)
            {
                faceManager = FindObjectOfType<ARFaceManager>();
            }
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            packet.face = info;
            info = null;
        }

        protected override void OnNdiTransformer(Material material)
        {

        }

        void OnFaceChanged(ARFacesChangedEventArgs args)
        {
            Debug.Log($"Face Changed: {args}");
            info = new ARKitRemotePacket.FaceInfo();

            info.added = args.added.Select(face => ToARFace(face)).ToArray();
            info.updated = args.updated.Select(face => ToARFace(face)).ToArray();
            info.removed = args.removed.Select(face => (ARKitStream.Internal.TrackableId)face.trackableId).ToArray();

            var meshes = new List<ARKitRemotePacket.FaceMesh>();
            meshes.AddRange(args.added.Select(face => ToMesh(face)));
            meshes.AddRange(args.updated.Select(face => ToMesh(face)));
            // meshes.AddRange(args.removed.Select(face => ToMesh(face)));
            info.meshes = meshes.ToArray();
        }

        static XRFace ToARFace(ARFace face)
        {
            return new XRFace()
            {
                trackableId = face.trackableId,
                pose = Pose.FromTransform(face.transform),
                trackingState = face.trackingState,
                nativePtr = face.nativePtr,
                leftEyePose = Pose.FromTransform(face.leftEye),
                rightEyePose = Pose.FromTransform(face.rightEye),
                fixationPoint = face.fixationPoint.localPosition,
            };
        }

        static ARKitRemotePacket.FaceMesh ToMesh(ARFace face)
        {
            var id = face.trackableId;
            return new ARKitRemotePacket.FaceMesh()
            {
                id = face.trackableId,
                // vertices = face.vertices.Select(v => (float3)v).ToArray(),
                // normals = face.normals.Select(n => (float3)n).ToArray(),
                // indices = face.indices.ToArray(),
                // uvs = face.uvs.Select(uv => (float2)uv).ToArray()
                vertices = face.vertices.ToRawBytes(),
                normals = face.normals.ToRawBytes(),
                indices = face.indices.ToRawBytes(),
                uvs = face.uvs.ToRawBytes(),
            };
        }

       
    }
}
