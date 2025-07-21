using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SoundCard : MonoBehaviour
{
    public SoundData SoundData => data;
    public bool IsOn => isOn;
    public bool IsFavorite => isFavorite;

    [SerializeField] private UnityEngine.UI.Image backgroundImage;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private UnityEngine.UI.Image isPremiumIcon;

    public Action<bool> OnFavoritesChanged;
    public Action<SoundData> OnAdd;
    public Action<SoundData> OnToggle;

    private SoundData data;
    private bool isOn;
    private bool isFavorite;

    public void Download(SoundData data)
    {
        this.data = data;
        title.text = data.title;

        isPremiumIcon.gameObject.SetActive(data.isPremium);

        // if we already have a sprite, use it immediately
        if (data.backgroundImage != null)
        {
            backgroundImage.overrideSprite = data.backgroundImage;
        }
        // otherwise download it, then assign to both the Image and the SoundData
        else if (!string.IsNullOrEmpty(data.backgroundImageUrl))
        {
            ImageExtensions.LoadSpriteFromURL(data.backgroundImageUrl, sprite =>
            {
                if (sprite != null)
                {
                    backgroundImage.overrideSprite = sprite;

                    backgroundImage.overrideSprite = sprite;
                    backgroundImage.type = UnityEngine.UI.Image.Type.Simple;
                    backgroundImage.preserveAspect = true;

                    // 4) Fit to full width, then recalc height to maintain original aspect
                    RectTransform rt = backgroundImage.rectTransform;
                    float targetWidth = rt.rect.width;
                    float newHeight = targetWidth * ((float)sprite.textureRect.height / sprite.textureRect.width);
                    rt.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Vertical,
                        newHeight
                    );

                    data.backgroundImage = sprite;
                }
            });
        }
    }

    public void HandleOnFavoritesChange(bool isOn)
    {
        OnFavoritesChanged?.Invoke(isOn);
        isFavorite = isOn;
    }
    public void HandleOnAdd()
    {
        OnAdd?.Invoke(data);
    }
    public void HandleOnOnToggle(bool isOn)
    {
        OnToggle?.Invoke(data);
        this.isOn = isOn;
    }
}
