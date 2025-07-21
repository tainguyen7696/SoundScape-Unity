// Assets/Scripts/PersistentDataManager.cs

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

#region Models

[Serializable]
public class PersistedSound
{
    public string title;
    public string audioUrl;
    public string backgroundImage;
    public SoundSettings settings;
}

[Serializable]
public class SoundSettings
{
    public float volume;
    public float warmth;
}

#endregion

/// <summary>
/// A singleton that loads & saves two lists (scenes & favorites) to PlayerPrefs as JSON.
/// Automatically loads on Awake, and re‑saves whenever you modify the data via its public methods.
/// </summary>
public class PersistentDataManager : MonoBehaviour
{
    private const string SceneKey = "MYAPP_CURRENT_SOUNDS";
    private const string FavoritesKey = "MYAPP_FAVORITES";

    public static PersistentDataManager Instance { get; private set; }

    /// <summary>
    /// The persisted list of sounds.
    /// </summary>
    public List<PersistedSound> Scene { get; private set; } = new List<PersistedSound>();

    /// <summary>
    /// The persisted list of favorites.
    /// </summary>
    public List<string> Favorites { get; private set; } = new List<string>();

    private void Awake()
    {
        // enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // load from PlayerPrefs
        LoadScene();
        LoadFavorites();
    }

    #region Scene Persistence

    private void LoadScene()
    {
        var json = PlayerPrefs.GetString(SceneKey, null);
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("🔄 No saved scene data found.");
            return;
        }

        try
        {
            Scene = JsonConvert.DeserializeObject<List<PersistedSound>>(json)
                    ?? new List<PersistedSound>();
            Debug.Log($"✅ Loaded {Scene.Count} sounds from cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to load scene data: {ex}");
            Scene = new List<PersistedSound>();
        }
    }

    private void SaveScene()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Scene);
            PlayerPrefs.SetString(SceneKey, json);
            PlayerPrefs.Save();
            Debug.Log($"💾 Saved {Scene.Count} sounds to cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to save scene data: {ex}");
        }
    }

    /// <summary>
    /// Replace the entire scene list and persist immediately.
    /// </summary>
    public void UpdateScene(List<PersistedSound> newScene)
    {
        Scene = newScene;
        SaveScene();
    }

    /// <summary>
    /// Add one sound and persist.
    /// </summary>
    public void AddSound(PersistedSound sound)
    {
        Scene.Add(sound);
        SaveScene();
    }

    /// <summary>
    /// Remove one sound (by reference) and persist.
    /// </summary>
    public void RemoveSound(PersistedSound sound)
    {
        if (Scene.Remove(sound))
            SaveScene();
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
