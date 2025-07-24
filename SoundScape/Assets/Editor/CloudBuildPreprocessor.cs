#if UNITY_CLOUD_BUILD
using UnityEditor;
using UnityEngine;
using UnityEngine.CloudBuild;

public static class CloudBuildPreprocessor
{
    // Unity Cloud Build will call this before exporting the Xcode project
    public static void PreExport(BuildManifestObject manifest)
    {
        // manifest.buildNumber is Cloud Build’s incrementing build number
        PlayerSettings.iOS.buildNumber = manifest.buildNumber;
        Debug.Log($"[CloudBuild] Setting iOS build number to {manifest.buildNumber}");
    }
}
#endif
