using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [Preserve]
    public class ARKitSessionRemoteSubsystem : XRSessionSubsystem
    {
        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_EDITOR
            const string id = "ARKit-Remote-Session";
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = id,
                subsystemImplementationType = typeof(ARKitSessionRemoteSubsystem),
                supportsInstall = false,
                supportsMatchFrameRate = false
            });
            Debug.Log($"Register {id} subsystem");
#endif // UNITY_EDITOR
        }

        class ARKitRemoteProvider : Provider
        {
            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                var flag = SessionAvailability.Supported | SessionAvailability.Installed;
                return Promise<SessionAvailability>.CreateResolvedPromise(flag);
            }

            public override TrackingState trackingState
            {
                get
                {
                    var receiver = ARKitReceiver.Instance;
                    if (receiver != null)
                    {
                        return receiver.trackingState;
                    }
                    return TrackingState.None;
                }
            }


        }
    }
}
