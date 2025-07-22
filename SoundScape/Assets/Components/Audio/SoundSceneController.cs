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

    public override void Awake()
    {
        base.Awake();
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
    private async void HandleSceneChanged(List<SceneItem> items)
    {
        if (items.Count == 0)
            isPlaying = false;
        try
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < items.Count)
                {
                    if (items[i].AudioClip == null)
                    {
                        var (clip, bytes) = await AudioExtensions.GetAudioClipWithBytesFromUrlAsync(items[i].SoundData.audioUrl);
                        AudioManager.Instance.PlayLayer(items[i].LayerIndex, clip);
                    }
                    else
                    {
                        AudioManager.Instance.PlayLayer(items[i].LayerIndex, items[i].AudioClip);
                    }

                    AudioManager.Instance.SetLayerVolume(items[i].LayerIndex, items[i].SoundData.settings.volume);
                    AudioManager.Instance.SetLayerWarmth(items[i].LayerIndex, items[i].SoundData.settings.warmth);
                }
            }

            foreach (var item in items)
            {

            }
        }
        catch { }

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
