using System.Runtime.InteropServices;
using UnityEngine;

class SilentModeOverride : MonoBehaviour
{
    [DllImport("__Internal", EntryPoint = "SilentModeOverride_Awake")]
    static extern void _NativeAwake();

    void Awake()
    {
        _NativeAwake();
    }
}
