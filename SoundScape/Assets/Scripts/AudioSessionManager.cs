using System.Runtime.InteropServices;
using UnityEngine;

public class AudioSessionManager : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _SetAudioSessionPlayback();
#endif

    void Awake()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _SetAudioSessionPlayback();
#endif
    }
}
