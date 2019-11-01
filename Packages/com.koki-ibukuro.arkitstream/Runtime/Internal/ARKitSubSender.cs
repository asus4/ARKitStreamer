using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitStream.Internal
{
    [RequireComponent(typeof(ARKitSender))]
    public abstract class ARKitSubSender : MonoBehaviour
    {

        protected virtual void OnEnable()
        {
            var sender = GetComponent<ARKitSender>();
            sender.PacketTransformer += OnPacketTransformer;
            sender.NdiTransformer += OnNdiTransformer;
        }

        protected virtual void OnDisable()
        {
            var sender = GetComponent<ARKitSender>();
            sender.PacketTransformer -= OnPacketTransformer;
            sender.NdiTransformer -= OnNdiTransformer;
        }

        protected virtual void OnPacketTransformer(ARKitRemotePacket packet)
        {
            // Override 
        }

        protected virtual void OnNdiTransformer(Material material)
        {
            // Override 
        }
    }
}