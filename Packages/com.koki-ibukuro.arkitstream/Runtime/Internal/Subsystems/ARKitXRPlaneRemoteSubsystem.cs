using System;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;
#elif MODULE_LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
#endif
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARKitStream.Internal
{
    [Preserve]
    public sealed class ARKitXRPlaneRemoteSubsystem : XRPlaneSubsystem
    {
        protected override Provider CreateProvider() => new ARKitRemoteProvider();

        class ARKitRemoteProvider : Provider
        {
            public override void Destroy() { }
            public override void Start() { }
            public override void Stop() { }
            public override TrackableChanges<BoundedPlane> GetChanges(
                BoundedPlane defaultPlane,
                Allocator allocator)
            {
                return default(TrackableChanges<BoundedPlane>);
             }

        }
    }
}
