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
    [SerializeField] private Toggle toggle;
    [SerializeField] private Button addButton;

    public Action<bool> OnFavoritesChanged;
    public Action<SoundData> OnAdd;
    public Action<SoundData> OnToggle;

    private SoundData data;
    private bool isOn;
    private bool isFavorite;

    void OnEnable()
    {
        IAPManager.OnPremiumUnlocked += OnPremiumUnlocked;
    }

    void OnDisable()
    {
        IAPManager.OnPremiumUnlocked -= OnPremiumUnlocked;
    }

    private void OnPremiumUnlocked()
    {
        ValidateIsUserPremium();
    }

    public void Download(SoundData data)
    {
        this.data = data;
        name = data.title;
        title.text = data.title;

        ValidateIsUserPremium();

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

        bool isFavorited = PersistentDataManager.Instance.Favorites.Any(x => x == data.title);
        favoriteToggle.SetIsOnWithoutNotify(isFavorited);
        this.isFavorite = isFavorited;
    }

    public void ValidateIsUserPremium()
    {
        bool isPremiumUser = IAPManager.Instance.IsPremiumUser();
        bool isPremiumContent = data.isPremium;

        isPremiumIcon.gameObject.SetActive(isPremiumContent && !isPremiumUser);
        toggle.interactable = !isPremiumContent || isPremiumUser;
        addButton.interactable = !isPremiumContent || isPremiumUser;
        favoriteToggle.interactable = !isPremiumContent || isPremiumUser;
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
    public void HandleLock()
    {
        SettingsPullup.Instance.SetActive(true);
    }
}
