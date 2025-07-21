using System;
using System.Collections.Generic;
using UnityEngine;

public class Scene : Singleton<Scene>
{
    public int Count => slots.Count;

    [SerializeField] private SceneItem sceneItemPrefab;
    [SerializeField] private Transform container;

    private readonly List<SceneItem> slots = new List<SceneItem>();

    public event Action<int> OnSceneChanged;

    public void AddSound(SoundData data)
    {
        if (slots.Count < 3)
        {
            var go = Instantiate(sceneItemPrefab.gameObject, container);
            var item = go.GetComponent<SceneItem>();
            slots.Add(item);
            item.Download(data);
            item.OnRemove += RemoveSound;
            OnSceneChanged?.Invoke(Count);
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
    }

    public void RemoveSound(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        var slot = slots[index];
        Destroy(slot.gameObject);
        slots.RemoveAt(index);
        OnSceneChanged?.Invoke(Count);
    }

    public void ClearScene()
    {
        foreach (var slot in slots)
            Destroy(slot.gameObject);
        slots.Clear();
        OnSceneChanged?.Invoke(Count);
    }
}
