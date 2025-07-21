using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SceneItem : MonoBehaviour
{
    public SoundData SoundData => data;

    private SoundData data;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Image backgroundImage;
    public Action<SceneItem> OnRemove;

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
    }

    public void HandleOnClick()
    {

    }

    public void HandleOnRemove()
    {
        OnRemove?.Invoke(this);
    }
}
