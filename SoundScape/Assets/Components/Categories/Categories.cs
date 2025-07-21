using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Categories : Factory<Category>
{
    [SerializeField] private Scene scene;
    public void Start()
    {
        Download();
    }

    public void Download()
    {
        DeleteAll();

        var allSounds = SoundDataManager.Instance.SoundDatas;

        var groups = allSounds
            .GroupBy(s => s.category)
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            Category category = Create();
            category.name = group.Key;
            category.Title = group.Key;
            foreach (var soundData in group)
            {
                SoundCard card = category.Create();
                card.Download(soundData);
                card.OnToggle += scene.ReplaceSound;
                card.OnAdd += scene.AddSound;
            }
        }
    }
}
