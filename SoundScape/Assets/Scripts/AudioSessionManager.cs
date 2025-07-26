using System.Runtime.InteropServices;
using UnityEngine;

public class AudioSessionManager : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _SetAudioSessionPlayback();
    [DllImport("__Internal")]
    private static extern void _SetAudioSessionBackground();
#endif

    void Awake()
    {
        // 1️⃣ keep Unity running when backgrounded
        Application.runInBackground = true;

        Debug.Log("[AudioSessionManager] Awake – configuring iOS audio session");

#if UNITY_IOS && !UNITY_EDITOR
        _SetAudioSessionPlayback();    // ignore Ring/Silent switch
        _SetAudioSessionBackground();  // allow background play
#endif
    }

    void OnApplicationPause(bool paused)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (paused)
        {
            Debug.Log("[AudioSessionManager] App paused – re‑applying background audio");
            _SetAudioSessionBackground();
        }
#endif
    }
}
