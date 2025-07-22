// Assets/Scripts/SoundDataManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Supabase;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Postgrest.Models;
using Postgrest.Attributes;
using UnityEngine.Networking;
using UnityEngine.U2D.Animation;

[Table("sounds")]
public class Sounds : BaseModel
{
    [PrimaryKey("id")][JsonProperty("id")] public string Id { get; set; }
    [Column("title")][JsonProperty("title")] public string Title { get; set; }
    [Column("audio_url")][JsonProperty("audio_url")] public string AudioUrl { get; set; }
    [Column("background_image_url")]
    [JsonProperty("background_image_url")] public string BackgroundImageUrl { get; set; }
    [Column("category")][JsonProperty("category")] public string Category { get; set; }
    [Column("is_premium")][JsonProperty("is_premium")] public bool IsPremium { get; set; }
    [Column("created_at")][JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
}

public class SoundDataManager : Singleton<SoundDataManager>
{
    public event Action<List<SoundData>> OnSoundDataImageLoaded;
    public event Action<List<SoundData>> OnSoundDataAudioClipLoaded;
    public event Action<List<SoundData>> OnSoundDataJsonLoaded;
    public IReadOnlyList<SoundData> SoundDatas => _soundDatas;

    const string FileName = "sound_data.json";
    const string AudioSubdir = "sound_audio";
    const string SpritesDir = "sound_sprites";

    string _filePath;
    string _spritesFolder;
    string _audioFolder;
    List<SoundData> _soundDatas = new List<SoundData>();

    public override async void Awake()
    {
        base.Awake();
        // prepare paths
        _filePath = Path.Combine(Application.persistentDataPath, FileName);
        _audioFolder = Path.Combine(Application.persistentDataPath, AudioSubdir);
        _spritesFolder = Path.Combine(Application.persistentDataPath, SpritesDir);

        Directory.CreateDirectory(_audioFolder);
        Directory.CreateDirectory(_spritesFolder);

        await CheckCacheThenLoadAsync();
    }

    private async UniTask CheckCacheThenLoadAsync()
    {
        // 1) If no local file, just refresh
        if (!File.Exists(_filePath))
        {
            await RefreshFromRemoteAsync();
            return;
        }

        // 2) Read local count
        List<SoundData> localList;
        try
        {
            var json = File.ReadAllText(_filePath);
            localList = JsonConvert.DeserializeObject<List<SoundData>>(json)
                        ?? new List<SoundData>();
        }
        catch
        {
            // corrupt local – force refresh
            await RefreshFromRemoteAsync();
            return;
        }

        // 3) Fetch remote count
        //    (we fetch metadata only; Supabase will still return the models)
        while (SupabaseManager.Client == null)
            await UniTask.Yield();

        var resp = await SupabaseManager.Client
                                    .From<Sounds>()
                                    .Select("title")
                                    .Get();
        int remoteCount = resp.Models?.Count ?? 0;

        // 4) Compare and load accordingly
        if (localList.Count == remoteCount)
            await LoadLocalAsync();
        else
            await RefreshFromRemoteAsync();
    }

    public void SaveSingleCache(SoundData item)
        => SaveSelectedCache(new[] { item });
    public void SaveSelectedCache(IEnumerable<SoundData> subset)
        => SaveJsonCache(subset.ToList());

    /// <summary>
    /// Force a fresh download from Supabase, overwriting cache.
    /// </summary>
    public async UniTask RefreshFromRemoteAsync()
    {
        // wait for Supabase client
        while (SupabaseManager.Client == null)
            await UniTask.Yield();

        try
        {
            Debug.Log("🔍 Fetching remote sound data...");
            var resp = await SupabaseManager.Client.From<Sounds>().Get();
            var list = resp.Models.Select(db => new SoundData
            {
                title = db.Title,
                audioUrl = db.AudioUrl,
                backgroundImageUrl = db.BackgroundImageUrl,
                category = db.Category,
                isPremium = db.IsPremium,
                createdAt = db.CreatedAt,
                settings = new SoundSettings(1f, 1f),
                backgroundImagePath = null
            }).ToList();


            _soundDatas = list;
            Debug.Log($"✅ Fetched {_soundDatas.Count} sounds from Supabase.");
            OnSoundDataJsonLoaded?.Invoke(_soundDatas);

            await LoadSpritesAndCacheAsync(list);
            OnSoundDataImageLoaded?.Invoke(_soundDatas);

            await LoadAudioClipsAndCacheAsync(_soundDatas);
            OnSoundDataAudioClipLoaded?.Invoke(_soundDatas);

            SaveJsonCache(_soundDatas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Remote refresh failed: {ex}");
        }
    }

    /// <summary>
    /// Load from local JSON + cached sprites on disk.
    /// </summary>
    private async UniTask LoadLocalAsync()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            _soundDatas = JsonConvert.DeserializeObject<List<SoundData>>(json)
                         ?? new List<SoundData>();

            Debug.Log($"✅ Loaded {_soundDatas.Count} soundDatas from local cache.");
            OnSoundDataJsonLoaded?.Invoke(_soundDatas);
            await LoadSpritesAndCacheAsync(_soundDatas);
            OnSoundDataImageLoaded?.Invoke(_soundDatas);
            await LoadAudioClipsAndCacheAsync(_soundDatas);
            OnSoundDataAudioClipLoaded?.Invoke(_soundDatas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to load local cache: {ex}");
            _soundDatas = new List<SoundData>();
            // fallback to remote if local is corrupt
            await RefreshFromRemoteAsync();
        }
    }

    /// <summary>
    /// Saves JSON only.
    /// </summary>
    private void SaveJsonCache(List<SoundData> list)
    {
        try
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            Debug.Log("💾 JSON cache saved.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed saving JSON cache: {ex}");
        }
    }

    /// <summary>
    /// Ensures each SoundData.backgroundImage is loaded (disk or remote),
    /// caches new downloads to disk, and updates backgroundImagePath.
    /// </summary>
    private async UniTask LoadSpritesAndCacheAsync(List<SoundData> list)
    {
        foreach (var sd in list)
        {
            // attempt disk load if path exists
            if (!string.IsNullOrEmpty(sd.backgroundImagePath))
            {
                string disk = Path.Combine(Application.persistentDataPath, sd.backgroundImagePath);
                if (File.Exists(disk))
                {
                    var data = File.ReadAllBytes(disk);
                    var tex = new Texture2D(2, 2);
                    if (tex.LoadImage(data))
                    {
                        sd.backgroundImage = Sprite.Create(
                            tex,
                            new Rect(0, 0, tex.width, tex.height),
                            Vector2.one * 0.5f
                        );
                        continue;
                    }
                }
            }

            // else remote download
            if (!string.IsNullOrEmpty(sd.backgroundImageUrl))
            {
                try
                {
                    sd.backgroundImage = await LoadRemoteSpriteAsync(sd.backgroundImageUrl);
                    // cache PNG to disk
                    CacheSpriteToDisk(sd);
                }
                catch (Exception imgEx)
                {
                    Debug.LogError($"❌ Sprite load failed ({sd.backgroundImageUrl}): {imgEx}");
                }
            }
        }
        foreach (var sd in list)
        {
            Debug.Log($"CACHED: SoundData sprite '{sd.title}': backgroundImagePath = '{sd.backgroundImagePath}'");
        }
        // update JSON with new backgroundImagePath values
        SaveJsonCache(list);
    }

    private async UniTask LoadAudioClipsAndCacheAsync(List<SoundData> list)
    {
        bool anyNew = false;

        foreach (var sd in list)
        {
            string safeName = string.Concat(sd.title.Split(Path.GetInvalidFileNameChars()));
            string ext = Path.GetExtension(sd.audioUrl).ToLower();
            string fname = $"{safeName}{ext}";
            string fullPath = Path.Combine(_audioFolder, fname);

            // 1) Try disk first
            var clip = await AudioExtensions.GetAudioFromDiskAsync(fullPath);
            if (clip != null)
            {
                sd.audioClip = clip;
                sd.audioClipPath = Path.Combine(AudioSubdir, fname);
            }
            else
            {
                // 2) Fallback to remote download
                var (downloadedClip, bytes) = await AudioExtensions
                                                 .GetAudioClipWithBytesFromUrlAsync(sd.audioUrl);

                if (downloadedClip != null && bytes != null)
                {
                    sd.audioClip = downloadedClip;
                    sd.audioClipPath = Path.Combine(AudioSubdir, fname);
                    anyNew = true;

                    // write file off the main thread
                    await UniTask.Run(() => File.WriteAllBytes(fullPath, bytes));
                }
                else
                {
                    Debug.LogError($"❌ Audio download failed: {sd.audioUrl}");
                }
            }

            Debug.Log($"CACHED: SoundData audio '{sd.title}': audioClipPath = '{sd.audioClipPath}'");
        }

        if (anyNew)
            SaveJsonCache(list);
    }

    private AudioType GetAudioType(string ext)
    {
        return ext switch
        {
            ".mp3" => AudioType.MPEG,
            ".mp4" => AudioType.MPEG,
            ".wav" => AudioType.WAV,
            ".ogg" => AudioType.OGGVORBIS,
            _ => AudioType.UNKNOWN,
        };
    }
    private UniTask<Sprite> LoadRemoteSpriteAsync(string url)
    {
        var tcs = new UniTaskCompletionSource<Sprite>();
        ImageExtensions.LoadSpriteFromURL(url, sprite =>
        {
            if (sprite != null) tcs.TrySetResult(sprite);
            else tcs.TrySetException(new Exception($"Null sprite at {url}"));
        });
        return tcs.Task;
    }

    private void CacheSpriteToDisk(SoundData sd)
    {
        if (sd.backgroundImage == null) return;

        try
        {
            string safe = string.Concat(sd.title.Split(Path.GetInvalidFileNameChars()));
            string fname = $"{safe}.png";
            string full = Path.Combine(_spritesFolder, fname);

            var tex = sd.backgroundImage.texture;
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(full, bytes);

            sd.backgroundImagePath = Path.Combine(SpritesDir, fname);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed caching sprite '{sd.title}': {ex}");
        }
    }

    /// <summary>
     /// Deletes all on‑disk caches (JSON, sprites, audio) and recreates empty folders.
     /// </summary>
    public void ClearAllCache()
    {
        // 1) Delete JSON cache
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
            Debug.Log("🗑️ Deleted JSON cache.");
        }

        // 2) Delete sprite cache
        if (Directory.Exists(_spritesFolder))
        {
            Directory.Delete(_spritesFolder, recursive: true);
            Debug.Log("🗑️ Deleted sprites cache.");
        }

        // 3) Delete audio cache
        if (Directory.Exists(_audioFolder))
        {
            Directory.Delete(_audioFolder, recursive: true);
            Debug.Log("🗑️ Deleted audio cache.");
        }

        // 4) Recreate folders so future loads won’t error
        Directory.CreateDirectory(_spritesFolder);
        Directory.CreateDirectory(_audioFolder);
        Debug.Log("✅ Recreated empty cache folders.");

        // 5) Clear in‑memory list if desired
        _soundDatas.Clear();
    }
}
