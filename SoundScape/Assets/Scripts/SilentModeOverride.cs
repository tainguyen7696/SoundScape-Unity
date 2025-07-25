using UnityEngine;
using System.Runtime.InteropServices;

public class SilentModeOverride : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    // Only DllImport when running on an actual iOS build
    [DllImport("__Internal")]
    private static extern void SilentModeOverride_Awake();
#else
    // Stub for Editor & other platforms
    private static void SilentModeOverride_Awake() { }
#endif

    void Awake()
    {
        SilentModeOverride_Awake();
    }
}
