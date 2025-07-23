using System.Collections.Generic;
using System.Linq;
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

    public AudioSource GetAudioSourceAt(int layerIndex)
    {
        return layerSources.ElementAt(layerIndex);
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

    public void ClearClipAt(int layerIndex)
    {
        var layerSource = layerSources.ElementAt(layerIndex);
        if (layerSource != null)
            layerSource.clip = null;
    }

    /// <summary>
    /// Stop playback on the specified layer.
    /// </summary>
    public void StopLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count) return;
        if (layerSources[layerIndex].isPlaying)
            layerSources[layerIndex].Stop();
    }

    /// <summary>
    /// Set per‑layer volume (0→1 maps to –80dB→0dB).
    /// </summary>
    public void SetLayerVolume(int layerIndex, float sliderValue)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count)
            return;

        float t = Mathf.Clamp01(sliderValue);

        float amplitude = t;
        float db;
        if (amplitude <= 0.0001f)
            db = -80f;
        else
            db = 20f * Mathf.Log10(amplitude);

        mixer.SetFloat($"Layer{layerIndex + 1}Volume", db);
    }


/// <summary>
/// Set per‑layer warmth (0→1 maps to –80dB→0dB).
/// </summary>
public void SetLayerWarmth(int layerIndex, float linearWarmth)
    {
        if (layerIndex < 0 || layerIndex >= layerSources.Count)
            return;

        float t = Mathf.Clamp01(linearWarmth);

        const float minCutoff = 200f;
        const float maxCutoff = 8000f;

        float logMin = Mathf.Log(minCutoff);
        float logMax = Mathf.Log(maxCutoff);
        float logCutoff = Mathf.Lerp(logMax, logMin, t);

        float cutoffHz = Mathf.Exp(logCutoff);

        mixer.SetFloat($"WarmthCutoff{layerIndex}", cutoffHz);
    }


    public void SetMasterVolume(float sliderValue)
    {
        float t = Mathf.Clamp01(sliderValue);

        float amplitude = t;
        float db;
        if (amplitude <= 0.0001f)
            db = -80f;
        else
            db = 20f * Mathf.Log10(amplitude);

        mixer.SetFloat("MasterVolume", db);
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
