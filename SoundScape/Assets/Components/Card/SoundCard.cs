using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SoundCard : MonoBehaviour
{
    public SoundData SoundData => data;
    public bool IsOn => isOn;
    public bool IsFavorite => isFavorite;
    public bool Outline
    {
        set
        {
            outline.enabled = value;
        }
    }

    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image isPremiumIcon;
    [SerializeField] private Image outline;
    [SerializeField] private Toggle favoriteToggle;

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

        if(data.backgroundImage != null)
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

        bool isFavorited = PersistentDataManager.Instance.Favorites.Any(x => x == data.title);
        favoriteToggle.SetIsOnWithoutNotify(isFavorited);
        this.isFavorite = isFavorited;
    }

    public void HandleOnFavoritesChange(bool isOn)
    {
        OnFavoritesChanged?.Invoke(isOn);
        if (isOn)
            PersistentDataManager.Instance.AddFavorite(data.title);
        else
            PersistentDataManager.Instance.RemoveFavorite(data.title);
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
