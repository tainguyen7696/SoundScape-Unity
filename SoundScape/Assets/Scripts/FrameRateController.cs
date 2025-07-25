// Assets/Scripts/FrameRateController.cs
using UnityEngine;

[DefaultExecutionOrder(-100)]  // run this very early
public class FrameRateController : MonoBehaviour
{
    void Awake()
    {
        // 1️⃣ Turn off V‑Sync so we can drive the framerate ourselves
        QualitySettings.vSyncCount = 0;

        // 2️⃣ Force a 60 FPS cap
        Application.targetFrameRate = 60;

        Debug.Log($"FrameRateController: vSync={QualitySettings.vSyncCount}, targetFrameRate={Application.targetFrameRate}");
    }
}
