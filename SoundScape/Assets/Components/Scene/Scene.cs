using Postgrest;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Scene : Singleton<Scene>
{
    public int Count => slots.Count;

    [SerializeField] private SceneItem sceneItemPrefab;
    [SerializeField] private Transform container;

    private readonly List<SceneItem> slots = new List<SceneItem>();

    public event Action<List<SceneItem>> OnSceneChanged;

    public void LoadPersistedScene()
    {
        foreach (var soundData in PersistentDataManager.Instance.persistentScene)
        {
            AddSound(soundData);
        }
    }

    public void AddSound(SoundData data)
    {
        if (slots.Count < 3)
        {
            var item = Instantiate(sceneItemPrefab, container);
            slots.Add(item);
            item.Download(data);
            item.OnRemove += RemoveSound;
            OnSceneChanged?.Invoke(slots);
        }
        else
        {
            ReplaceSound(data);
        }
    }

    public void ReplaceSound(SoundData data)
    {
        if (slots.Count == 0)
        {
            AddSound(data);
            return;
        }

        var item = slots[slots.Count - 1];
        item.Download(data);
        item.gameObject.SetActive(true);
        OnSceneChanged?.Invoke(slots);
    }

    public void RemoveSound(SceneItem item)
    {
        if (slots.Contains(item))
        {
            slots.Remove(item);
            Destroy(item.gameObject);
        }
        OnSceneChanged?.Invoke(slots);
    }

    public void ClearScene()
    {
        foreach (var slot in slots)
            Destroy(slot.gameObject);
        slots.Clear();
        OnSceneChanged?.Invoke(slots);
    }
}
