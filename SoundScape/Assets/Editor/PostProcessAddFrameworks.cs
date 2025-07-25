// Assets/Editor/PostProcessAddFrameworks.cs
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class PostProcessAddFrameworks
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;
        // locate the project
        var projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        // iOS 14+ uses separate UnityFramework target
        var unityFrameworkTarget = proj.GetUnityFrameworkTargetGuid();
        // link AVFoundation.framework
        proj.AddFrameworkToProject(unityFrameworkTarget, "AVFoundation.framework", false);
        // (if you also need AudioToolbox, uncomment:)
        // proj.AddFrameworkToProject(unityFrameworkTarget, "AudioToolbox.framework", false);
        // save
        proj.WriteToFile(projPath);
        UnityEngine.Debug.Log("[PostProcess] Linked AVFoundation.framework");
    }
}
