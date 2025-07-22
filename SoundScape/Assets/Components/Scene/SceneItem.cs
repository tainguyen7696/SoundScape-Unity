using System;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SceneItem : MonoBehaviour
{
    public int LayerIndex {
        get => layerIndex;
        set => layerIndex = value; 
    }
    public AudioClip AudioClip => audioClip;
    public SoundData SoundData => data;

    private SoundData data;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Image backgroundImage;
    public Action<SceneItem> OnRemove;

    private int layerIndex;
    private AudioClip audioClip;

    public void Download(SoundData data)
    {
        this.data = data;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = true;

        if (data.backgroundImage != null)
        {
            backgroundImage.overrideSprite = data.backgroundImage;
            backgroundImage.enabled = true;
        }
        else
        {
            ImageExtensions.LoadSpriteFromURL(data.backgroundImageUrl, downloadedSprite =>
            {
                if (downloadedSprite != null)
                {
                    backgroundImage.overrideSprite = downloadedSprite;

                    backgroundImage.enabled = true;
                    data.backgroundImage = downloadedSprite;
                }
            });
        }

        if (data.audioClip != null)
        {
            this.audioClip = data.audioClip;
        }
        else
        {
            var (clip, bytes) = AudioExtensions.GetAudioClipWithBytesFromUrl(data.audioUrl);

            if (clip != null && bytes != null)
            {
                data.audioClip = clip;
            }
            else
            {
                Debug.LogError($"❌ Audio download failed: {data.audioUrl}");
            }
        }
    }

    public void HandleOnClick()
    {
        SoundSettingsPullup.Instance.Download(this);
        SoundSettingsPullup.Instance.SetActive(true);
    }

    public void HandleOnRemove()
    {
        OnRemove?.Invoke(this);
    }
}
