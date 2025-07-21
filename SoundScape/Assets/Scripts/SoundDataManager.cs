using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Supabase;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Postgrest.Models;
using Postgrest.Attributes;

public class SoundData
{
    [JsonProperty("title")]
    public string title { get; set; }

    [JsonProperty("audioUrl")]
    public string audioUrl { get; set; }

    [JsonProperty("backgroundImageUrl")]
    public string backgroundImageUrl { get; set; }

    [JsonIgnore]
    public Sprite backgroundImage { get; set; }

    [JsonProperty("category")]
    public string category { get; set; }

    [JsonProperty("isPremium")]
    public bool isPremium { get; set; }

    [JsonProperty("createdAt")]
    public DateTime createdAt { get; set; }
}

[Table("sounds")]
public class Sounds : BaseModel
{
    [PrimaryKey("id")]
    [JsonProperty("id")]
    public string Id { get; set; }

    [Column("title")]
    [JsonProperty("title")]
    public string Title { get; set; }

    [Column("audio_url")]
    [JsonProperty("audio_url")]
    public string AudioUrl { get; set; }

    [Column("background_image_url")]
    [JsonProperty("background_image_url")]
    public string BackgroundImageUrl { get; set; }

    [Column("category")]
    [JsonProperty("category")]
    public string Category { get; set; }

    [Column("is_premium")]
    [JsonProperty("is_premium")]
    public bool IsPremium { get; set; }

    [Column("created_at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
}

public class SoundDataManager : Singleton<SoundDataManager>
{
    // Other scripts can subscribe to be notified whenever SoundData is loaded/refreshed.
    public event Action<List<SoundData>> OnSoundDataLoaded;

    public IReadOnlyList<SoundData> SoundDatas => _soundDatas;
    private List<SoundData> _soundDatas = new List<SoundData>();

    private const string FileName = "sound_data.json";
    private string _filePath;

    async void Awake()
    {
        _filePath = Path.Combine(Application.persistentDataPath, FileName);

        // 1) Load local cache if it exists
        if (File.Exists(_filePath))
            await LoadLocalDataAsync();

        // 2) Then always fetch fresh from remote
        await RefreshFromRemoteAsync();
    }

    private async UniTask LoadLocalDataAsync()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            _soundDatas = JsonConvert
                .DeserializeObject<List<SoundData>>(json)
                ?? new List<SoundData>();

            Debug.Log($"✅ Loaded {_soundDatas.Count} SoundData from local cache.");

            // hydrate sprites
            await LoadSpritesForAllAsync(_soundDatas);

            // notify listeners
            OnSoundDataLoaded?.Invoke(_soundDatas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to load local SoundData: {ex.Message}");
            _soundDatas = new List<SoundData>();
        }
    }

    private async UniTask RefreshFromRemoteAsync()
    {
        // wait for Supabase client
        while (SupabaseManager.Client == null)
            await UniTask.Yield();

        try
        {
            Debug.Log("🔍 Fetching remote sound data...");
            var resp = await SupabaseManager.Client
                .From<Sounds>()
                .Get();

            var remote = resp.Models;
            var list = new List<SoundData>(remote.Count);

            foreach (var s in remote)
            {
                list.Add(new SoundData
                {
                    title = s.Title,
                    audioUrl = s.AudioUrl,
                    backgroundImageUrl = s.BackgroundImageUrl,
                    category = s.Category,
                    isPremium = s.IsPremium,
                    createdAt = s.CreatedAt
                });
            }

            // hydrate sprites
            await LoadSpritesForAllAsync(list);

            _soundDatas = list;
            Debug.Log($"✅ Fetched {_soundDatas.Count} SoundData from Supabase.");

            SaveLocalData();

            // notify listeners
            OnSoundDataLoaded?.Invoke(_soundDatas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error fetching remote SoundData: {ex.Message}");
        }
    }

    private void SaveLocalData()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_soundDatas, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            Debug.Log($"💾 Saved {_soundDatas.Count} SoundData to local cache.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to save SoundData: {ex.Message}");
        }
    }

    private async UniTask LoadSpritesForAllAsync(List<SoundData> list)
    {
        foreach (var sd in list)
        {
            if (!string.IsNullOrEmpty(sd.backgroundImageUrl))
            {
                try
                {
                    sd.backgroundImage = await LoadSpriteAsync(sd.backgroundImageUrl);
                }
                catch (Exception imgEx)
                {
                    Debug.LogError($"❌ Failed to load sprite ({sd.backgroundImageUrl}): {imgEx.Message}");
                }
            }
        }
        Debug.Log($"🔄 Loaded sprites for {list.Count} SoundData entries.");
    }

    private UniTask<Sprite> LoadSpriteAsync(string url)
    {
        var tcs = new UniTaskCompletionSource<Sprite>();
        ImageExtensions.LoadSpriteFromURL(url, sprite =>
        {
            if (sprite != null) tcs.TrySetResult(sprite);
            else tcs.TrySetException(new Exception($"Sprite null for {url}"));
        });
        return tcs.Task;
    }
}
