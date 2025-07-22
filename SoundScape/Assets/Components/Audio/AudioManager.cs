using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer & Layer Groups")]
    [Tooltip("Your AudioMixer_Main asset")]
    [SerializeField] private AudioMixer mixer;
    [Tooltip("Assign Layer1, Layer2, Layer3 mixer groups here (in order)")]
    [SerializeField] private AudioMixerGroup[] layerGroups = new AudioMixerGroup[3];

    // One AudioSource per layer
    public List<AudioSource> layerSources = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start looping this clip on the specified layer (0‑2). Replaces any previous clip on that layer.
    /// </summary>
    public void PlayLayer(int layerIndex, AudioClip clip)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count) return;
        var src = layerSources[layerIndex];
        src.clip = clip;
        src.Play();
    }

    public void ResetClips()
    {
        foreach (var layerSource in layerSources)
        {
            layerSource.clip = null;
        }
    }

    /// <summary>
    /// Stop playback on the specified layer.
    /// </summary>
    public void StopLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count) return;
        layerSources[layerIndex].Stop();
    }

    /// <summary>
    /// Set per‑layer volume (0→1 maps to –80dB→0dB).
    /// </summary>
    public void SetLayerVolume(int layerIndex, float linearVolume)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count) return;
        float dB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(linearVolume));
        mixer.SetFloat($"Layer{layerIndex + 1}Volume", dB);
    }

    /// <summary>
    /// Set per‑layer warmth (0→1 maps to –80dB→0dB).
    /// </summary>
    public void SetLayerWarmth(int layerIndex, float linearWarmth)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count) return;
        float cutoffHz = Mathf.Lerp(500f, 22000f, Mathf.Clamp01(linearWarmth));
        mixer.SetFloat("WarmthCutoff" + layerIndex, cutoffHz);
    }

    /// <summary>
    /// Set master volume (0→1 maps to –80dB→0dB).
    /// </summary>
    public void SetMasterVolume(float linearVolume)
    {
        float dB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(linearVolume));
        mixer.SetFloat("MasterVolume", dB);
    }

    public void Pause()
    {
        foreach (var layer in layerSources)
        {
            layer.Pause();
        }
    }

    public void Resume()
    {
        foreach (var layer in layerSources)
        {
            layer.UnPause();
        }
    }
}
