#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class iOSPostProcess
{
    // This method is called by Unity after the Xcode project is generated
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        // Locate the Info.plist file
        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        var pl = new PlistDocument();
        pl.ReadFromFile(plistPath);

        // Add or merge UIBackgroundModes → audio
        var root = pl.root;
        PlistElement existing = root.values.ContainsKey("UIBackgroundModes")
            ? root["UIBackgroundModes"]
            : null;

        PlistElementArray bgModes = existing is PlistElementArray arr
            ? arr
            : root.CreateArray("UIBackgroundModes");

        // Only add if not already present
        bool hasAudio = false;
        foreach (var e in bgModes.values)
            if (e.AsString() == "audio") { hasAudio = true; break; }
        if (!hasAudio)
            bgModes.AddString("audio");

        // Write changes back to Info.plist
        File.WriteAllText(plistPath, pl.WriteToString());
        Debug.Log("✅ Injected UIBackgroundModes: audio into Info.plist");
    }
}
#endif
