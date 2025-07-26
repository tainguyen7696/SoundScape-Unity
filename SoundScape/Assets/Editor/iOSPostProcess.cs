#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.IO;
using System.Linq;

public static class iOSPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS) return;
        var plistPath = Path.Combine(path, "Info.plist");
        var doc = new PlistDocument();
        doc.ReadFromFile(plistPath);

        // create or fetch the array
        PlistElementArray modes = doc.root.values.TryGetValue("UIBackgroundModes", out var existing)
            && existing is PlistElementArray arr ? arr
            : doc.root.CreateArray("UIBackgroundModes");

        // add “audio” if it’s missing
        if (!modes.values.Any(e => e.AsString() == "audio"))
            modes.AddString("audio");

        // WRITE BACK
        File.WriteAllText(plistPath, doc.WriteToString());

        // LOG what’s in there now
        var list = modes.values.Select(e => e.AsString()).ToArray();
        Debug.Log($"✅ Info.plist UIBackgroundModes entries: {string.Join(", ", list)}");
    }
}
#endif
