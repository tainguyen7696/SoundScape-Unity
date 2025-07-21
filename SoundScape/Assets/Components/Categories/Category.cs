using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CategoryData
{
    public string title;
    public List<Sounds> soundDatas;
}


public class Category : Factory<SoundCard>
{
    public string Title
    {
        set => title.text = value;
    }
    [Space]
    [SerializeField] private TextMeshProUGUI title;
}
