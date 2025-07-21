using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : Singleton<ColorManager>
{
    [SerializeField] private bool isLight;
    [SerializeField] private List<ThemeSO> themes;
    private ThemeSO theme;
    public Color Background => theme.background;
    public Color background2 => theme.background2;
    public Color CardBackground => theme.cardBackground;
    public Color Text => theme.text;
    public Color TextDim => theme.textDim;
    public Color Primary => theme.primary;
    public Color Overlay => theme.overlay;

    public List<Image> backgrounds = new List<Image>();
    public List<Image> background2s = new List<Image>();
    public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

    public override void Awake()
    {
        base.Awake();
        theme = isLight ? themes.ElementAt(0) : themes.ElementAt(1);
    }
}
