using UnityEngine;
using System.Runtime.InteropServices;

public class SilentModeOverride : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _OverrideAudioSessionToPlayback();
#endif

    void Awake()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _OverrideAudioSessionToPlayback();
#endif
    }
}
