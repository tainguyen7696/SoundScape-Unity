// Assets/Scripts/PersistentDataManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Rendering;


/// <summary>
/// A singleton that loads & saves two lists (scenes & favorites) to PlayerPrefs as JSON.
/// Automatically loads on Awake, and re‑saves whenever you modify the data via its public methods.
/// </summary>
public class PersistentDataManager : Singleton<PersistentDataManager>
{
    private const string SceneKey = "MYAPP_CURRENT_SOUNDS";
    private const string FavoritesKey = "MYAPP_FAVORITES";

    public List<SoundData> persistentScene { get; private set; } = new List<SoundData>();
    public List<string> Favorites { get; private set; } = new List<string>();

    public override void Awake()
    {
        base.Awake();

        SoundDataManager.Instance.OnSoundDataJsonLoaded += Instance_OnSoundDataJsonLoaded;
        Scene.Instance.OnSceneChanged += Instance_OnSceneChanged;
    }

    private void Instance_OnSoundDataJsonLoaded(List<SoundData> obj)
    {
        LoadScene();
        LoadFavorites();
    }

    private void Instance_OnSceneChanged(List<SceneItem> sceneItems)
    {
        persistentScene.Clear();
        foreach (var sceneItem in sceneItems)
        {
            persistentScene.Add(sceneItem.SoundData);
        }

        // Replace the entire persisted list
        SaveScene();
    }

    #region Scene Persistence

    private void LoadScene()
    {
        string json = PlayerPrefs.GetString(SceneKey, null);
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("🔄 No saved scene data found.");
            return;
        }

        try
        {
            // 1) Deserialize into a list of “stubs”
            var savedList = JsonConvert.DeserializeObject<List<SoundData>>(json)
                                ?? new List<SoundData>();

            // 2) For each stub, try to find the real instance in SoundDataManager by title
            persistentScene = savedList
                .Select(stub =>
                    SoundDataManager.Instance.SoundDatas
                        .FirstOrDefault(x => x.title == stub.title)
                    // if no match, fall back to the stub itself
                    ?? stub
                )
                .ToList();

            // 3) Hand it off to your scene loader
            Scene.Instance.LoadPersistedScene();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load scene data: {ex}");
            persistentScene = new List<SoundData>();
        }
    }


    private void SaveScene()
    {
        try
        {
            var json = JsonConvert.SerializeObject(persistentScene);
            PlayerPrefs.SetString(SceneKey, json);
            PlayerPrefs.Save();
            Debug.Log($"💾 Saved {persistentScene.Count} sounds to cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to save scene data: {ex}");
        }
    }

    #endregion

    #region Favorites Persistence

    private void LoadFavorites()
    {
        var json = PlayerPrefs.GetString(FavoritesKey, null);
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("🔄 No saved favorites found.");
            return;
        }

        try
        {
            Favorites = JsonConvert.DeserializeObject<List<string>>(json)
                        ?? new List<string>();
            Debug.Log($"✅ Loaded {Favorites.Count} favorites from cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to load favorites: {ex}");
            Favorites = new List<string>();
        }
    }

    private void SaveFavorites()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Favorites);
            PlayerPrefs.SetString(FavoritesKey, json);
            PlayerPrefs.Save();
            Debug.Log($"💾 Saved {Favorites.Count} favorites to cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to save favorites: {ex}");
        }
    }

    /// <summary>
    /// Add a favorite (if not already present) and persist.
    /// </summary>
    public void AddFavorite(string item)
    {
        if (!Favorites.Contains(item))
        {
            Favorites.Add(item);
            SaveFavorites();
        }
    }

    /// <summary>
    /// Remove a favorite and persist.
    /// </summary>
    public void RemoveFavorite(string item)
    {
        if (Favorites.Remove(item))
            SaveFavorites();
    }

    #endregion
}
