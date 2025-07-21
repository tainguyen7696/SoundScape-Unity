// Factory.cs
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// A generic factory MonoBehaviour: drag‐in your prefab, then call Create() to spawn an instance.
/// </summary>
/// <typeparam name="T">The Component type on your prefab.</typeparam>
public class Factory<T> : MonoBehaviour where T : Component
{
    public List<T> Objs => objs;

    [Header("Factory")]
    [SerializeField] private T prefab;
    [SerializeField] private Transform folder;
    [SerializeField] private List<T> objs = new List<T>();

    /// <summary>
    /// Instantiates the prefab as a child of this Factory’s GameObject and returns the new T component.
    /// </summary>
    public T Create()
    {
        if (prefab == null)
        {
            Debug.LogError($"Factory<{typeof(T).Name}>: Prefab is null!");
            return null;
        }

        T instance = Instantiate(prefab, folder);
        objs.Add(instance);
        return instance;
    }

    /// <summary>
    /// Destroys the given instance (if it was spawned by this factory) and removes it from tracking.
    /// </summary>
    public void Delete(T instance)
    {
        if (instance == null)
            return;

        if (objs.Remove(instance))
        {
            Destroy(instance.gameObject);
        }
        else
        {
            Debug.LogWarning($"Factory<{typeof(T).Name}>: Instance not tracked, destroying anyway.");
            Destroy(instance.gameObject);
        }
    }

    /// <summary>
    /// Destroys all spawned instances and clears the tracking list.
    /// </summary>
    public void DeleteAll()
    {
        // iterate backwards in case Destroy triggers any cleanup
        for (int i = objs.Count - 1; i >= 0; i--)
        {
            T inst = objs[i];
            if (inst != null)
                Destroy(inst.gameObject);
        }
        objs.Clear();
    }
}
