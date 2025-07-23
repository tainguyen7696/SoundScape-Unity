using Postgrest;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Scene : Singleton<Scene>
{
    public int Count => slots.Count;
    public List<SceneItem> SceneItems => slots;

    [SerializeField] private SceneItem sceneItemPrefab;
    [SerializeField] private Transform container;

    private readonly List<SceneItem> slots = new List<SceneItem>();

    public event Action<List<SceneItem>> OnSceneChanged;

    private void OnDestroy()
    {
        DeleteAllSoundGameObjects();
    }

    public void LoadPersistedScene()
    {
        foreach (var soundData in PersistentDataManager.Instance.persistentScene)
        {
            AddSound(soundData);
        }
        SoundSceneController.Instance.LoadPersistedSound();
    }

    public void AddSound(SoundData data)
    {
        if (slots.Count < 3)
        {
            var item = Instantiate(sceneItemPrefab, container);
            slots.Add(item);
            item.LayerIndex = slots.Count - 1;
            item.Download(data);
            item.OnRemove += RemoveSound;
            OnSceneChanged?.Invoke(slots);
        }
        else
        {
            ReplaceSound(data);
        }
        SoundSceneController.Instance.HandlePlayPause(true);
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

        SoundSceneController.Instance.HandlePlayPause(true);
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

    public void DeleteAllSoundGameObjects()
    {
        foreach (var item in slots)
        {
            Destroy(item.gameObject);
        }
        slots.Clear();
    }
}
