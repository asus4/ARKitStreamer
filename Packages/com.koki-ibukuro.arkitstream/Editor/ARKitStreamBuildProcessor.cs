using System.Linq;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;

namespace ARKitStream
{
    public class ARKitStreamBuildProcessor : XRBuildHelper<ARKitStreamSetting>
    {
        public override string BuildSettingsKey => "com.github.asus4.arkitstream.setting";

        public override void OnPreprocessBuild(BuildReport report)
        {
            base.OnPreprocessBuild(report);
        }

        public override void OnPostprocessBuild(BuildReport report)
        {
            base.OnPostprocessBuild(report);
        }
    }
}
