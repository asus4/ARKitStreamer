using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;

using ARKitStream.Internal;

namespace ARKitStream
{
    public class ARKitPlaneSender : ARKitSubSender
    {
        [SerializeField] ARPlaneManager planeManager = null;

        protected override void Start()
        {
            base.Start();
            Debug.Log("Setup ARKitPlaneSender");
            planeManager.planesChanged += OnPlaneChanged;
        }

        protected override void OnDestroy()
        {
            planeManager.planesChanged -= OnPlaneChanged;
            base.OnDestroy();
        }

        void OnValidate()
        {
            if (planeManager == null)
            {
                planeManager = FindObjectOfType<ARPlaneManager>();
            }
        }

        void OnPlaneChanged(ARPlanesChangedEventArgs args)
        {
            var added = args.added;
            var updated = args.updated;
            var removed = args.removed;

            Debug.Log($"added: {added.Count}, updated: {updated.Count}, removed: {removed.Count}");
        }

        public static ARKitPlaneSender TryCreate(ARKitSender sender)
        {
            ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitPlaneSender>();
            self.planeManager = planeManager;
            return self;
        }
    }
}
