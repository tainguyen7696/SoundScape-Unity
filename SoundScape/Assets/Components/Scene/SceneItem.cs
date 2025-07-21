using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneItem : MonoBehaviour
{
    public SoundData SoundData => data;

    private SoundData data;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Image backgroundImage;
    public Action<int> OnRemove;

    public void Download(SoundData data)
    {
        backgroundImage.overrideSprite = data.backgroundImage;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = true;

        // 4) Fit to full width, then recalc height to maintain original aspect
        RectTransform rt = backgroundImage.rectTransform;
        float targetWidth = rt.rect.width;
        float newHeight = targetWidth * ((float)data.backgroundImage.textureRect.height / data.backgroundImage.textureRect.width);
        rt.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            newHeight
        );
    }

    public void HandleOnClick()
    {

    }

    public void HandleOnRemove()
    {
        OnRemove?.Invoke(transform.GetSiblingIndex());
    }
}
