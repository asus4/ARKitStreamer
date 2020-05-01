using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;

namespace ARKitStream
{
    [AddComponentMenu("")]    
    public class ARKitHumanBodySender : ARKitSubSender
    {
        [SerializeField] ARHumanBodyManager humanBodyManager = null;

        ARKitRemotePacket.HumanBodyInfo info = null;

        protected override void Start()
        {
            base.Start();
            humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
        }

        protected override void OnPacketTransformer(ARKitRemotePacket packet)
        {
            packet.humanBody = info;
            info = null;
        }

        private void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs args)
        {
            info = new ARKitRemotePacket.HumanBodyInfo();

            info.added = args.added.Select(humanBody => ToHumanBody(humanBody)).ToArray();
            info.updated = args.updated.Select(humanBody => ToHumanBody(humanBody)).ToArray();
            info.removed = args.removed.Select(humanBody => (ARKitStream.Internal.TrackableId)humanBody.trackableId).ToArray();

            var joints = new Dictionary<TrackableId, byte[]>();
            foreach (var humanBody in args.added)
            {
                joints.Add(humanBody.trackableId, humanBody.joints.ToRawBytes());
            }
            foreach (var humanBody in args.updated)
            {
                joints.Add(humanBody.trackableId, humanBody.joints.ToRawBytes());
            }
            info.joints = joints;
        }

        static XRHumanBody ToHumanBody(ARHumanBody body)
        {
            return new XRHumanBody()
            {
                trackableId = body.trackableId,
                pose = body.pose,
                estimatedHeightScaleFactor = body.estimatedHeightScaleFactor,
                trackingState = body.trackingState,
                nativePtr = IntPtr.Zero,
            };
        }

        public static ARKitHumanBodySender TryCreate(ARKitSender sender)
        {
            var manager = FindObjectOfType<ARHumanBodyManager>();
            if (manager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitHumanBodySender>();
            self.humanBodyManager = manager;
            return self;
        }
    }
}