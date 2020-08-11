using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using ARKitStream.Internal;

namespace ARKitStream
{
    [AddComponentMenu("")]
    public class ARKitEnvironmentProbeSender : ARKitSubSender
    {
        [SerializeField] AREnvironmentProbeManager environmentProbeManager = null;

        protected override void Start()
        {
            base.Start();
            environmentProbeManager.environmentProbesChanged += OnEnvironmentProbesChanged;
        }

        protected override void OnDestroy()
        {
            environmentProbeManager.environmentProbesChanged -= OnEnvironmentProbesChanged;
            base.OnDestroy();
        }

        private void OnEnvironmentProbesChanged(AREnvironmentProbesChangedEvent e)
        {
            Debug.Log($"env probes: {e}");

            for (int i = 0; i < e.added.Count; i++)
            {
                Debug.Log($"[added {i}] {ToString(e.added[i])}");
            }
            for (int i = 0; i < e.updated.Count; i++)
            {
                Debug.Log($"[updated {i}] {ToString(e.updated[i])}");
            }
        }

        static string ToString(AREnvironmentProbe probe)
        {
            return $"{probe.trackableId}: {probe.textureDescriptor}";
        }

        public static ARKitEnvironmentProbeSender TryCreate(ARKitSender sender)
        {
            var manager = FindObjectOfType<AREnvironmentProbeManager>();
            if (manager == null)
            {
                return null;
            }
            var self = sender.gameObject.AddComponent<ARKitEnvironmentProbeSender>();
            self.environmentProbeManager = manager;
            return self;
        }
    }

}
