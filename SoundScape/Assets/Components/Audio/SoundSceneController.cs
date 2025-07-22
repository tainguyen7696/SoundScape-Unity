// Assets/Scripts/SoundScene.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSceneController : Singleton<SoundSceneController>
{
    [Header("UI Elements")]
    [SerializeField] private Toggle playPauseToggle;
    [SerializeField] private Image playIcon, pauseIcon;
    [SerializeField] private Slider masterVolumeSlider;

    private bool isPlaying = true;

    private void Awake()
    {
        Scene.Instance.OnSceneChanged += HandleSceneChanged;
    }

    private void OnDestroy()
    {
        // Clean up subscription
        if (Scene.Instance != null)
            Scene.Instance.OnSceneChanged -= HandleSceneChanged;
    }

    public void LoadPersistedSound()
    {
        HandleSceneChanged(Scene.Instance.SceneItems);
    }

    /// <summary>
    /// Called when the scene changes.
    /// </summary>
    private void HandleSceneChanged(List<SceneItem> items)
    {
        if (items.Count == 0)
            isPlaying = false;
        AudioManager.Instance.ResetClips();
        foreach (var item in items)
        {
            if (item.AudioClip == null)
            {
                var (clip, bytes) = AudioExtensions.GetAudioClipWithBytesFromUrl(item.SoundData.audioUrl);
                AudioManager.Instance.PlayLayer(item.LayerIndex, clip);
            }
            else
            {
                AudioManager.Instance.PlayLayer(item.LayerIndex, item.AudioClip);
            }
        }

        if (!isPlaying)
            AudioManager.Instance.Pause();
    }

    /// <summary>
    /// Toggle between playing and paused.
    /// </summary>
    public void HandlePlayPause(bool isPlaying)
    {
        this.isPlaying = isPlaying;

        if (isPlaying)
            AudioManager.Instance.Resume();
        else
            AudioManager.Instance.Pause();
        playPauseToggle.SetIsOnWithoutNotify(isPlaying);
        playIcon.enabled = !isPlaying;
        pauseIcon.enabled = isPlaying;
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.SetMasterVolume(volume);
    }

    public void SetLayerVolume(int layerIndex, float volume)
    {
        AudioManager.Instance.SetLayerVolume(layerIndex, volume);
    }
    public void SetLayerWarmth(int layerIndex, float warmth)
    {
        AudioManager.Instance.SetLayerWarmth(layerIndex, warmth);
    }

    public void PlayLayer(int layerIndex, AudioClip clip)
    {
        AudioManager.Instance.PlayLayer(layerIndex, clip);
    }

    public void StopLayer(int layerIndex)
    {
        AudioManager.Instance.StopLayer(layerIndex);
    }
}
