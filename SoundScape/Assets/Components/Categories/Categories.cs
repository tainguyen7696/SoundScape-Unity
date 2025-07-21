using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Categories : Factory<Category>
{

    public void Awake()
    {
        SoundDataManager.Instance.OnSoundDataJsonLoaded += Instance_OnSoundDataJsonLoaded;
        Scene.Instance.OnSceneChanged += Instance_OnSceneChanged;
    }

    private void Instance_OnSoundDataJsonLoaded(List<SoundData> obj)
    {
        Download();
    }

    private void OnDestroy()
    {
        if (Scene.Instance != null)
            Scene.Instance.OnSceneChanged -= Instance_OnSceneChanged;
    }

    private void Instance_OnSceneChanged(List<SceneItem> sceneItems)
    {
        // 1) Disable all outlines
        foreach (var category in Objs)
        {
            foreach (SoundCard card in category.Objs)
            {
                card.Outline = false;
            }
        }

        foreach (var item in sceneItems)
        {
            string title = item.SoundData.title;

            foreach (var category in Objs)
            {
                var soundCard = category.Objs
                    .FirstOrDefault(x => x.SoundData.title.ToLowerInvariant() == title.ToLowerInvariant());

                if (soundCard != null)
                {
                    soundCard.Outline = true;
                    break;
                }
            }
        }
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
            Category category = CreateOrGet(group.Key);
            category.name = group.Key;
            category.Title = group.Key;
            foreach (var soundData in group)
            {
                SoundCard card = category.CreateOrGet(soundData);
                card.OnToggle = null;
                card.OnAdd = null;
                card.OnToggle += Scene.Instance.ReplaceSound;
                card.OnAdd += Scene.Instance.AddSound;
            }
        }
    }

    public Category CreateOrGet(string title)
    {
        var obj = Objs.FirstOrDefault(x => x.Title == title);
        if (obj == null)
        {
            obj = Create();
            obj.Title = title;
        }
        return obj;
    }
}
