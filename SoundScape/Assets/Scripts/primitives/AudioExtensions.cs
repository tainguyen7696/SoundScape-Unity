// Assets/Scripts/AudioExtensions.cs
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public static class AudioExtensions
{
    /// <summary>
    /// Attempts to load an AudioClip from a local file on disk.
    /// Returns null if the file doesn't exist or fails to load.
    /// Decoding is streamed off the main thread.
    /// </summary>
    /// <param name="fullPath">Absolute path on disk (no "file://" prefix)</param>
    public static async UniTask<AudioClip> GetAudioFromDiskAsync(string fullPath)
    {
        if (!File.Exists(fullPath))
            return null;

        string fileUri = "file://" + fullPath;
        string ext = Path.GetExtension(fullPath).ToLower();
        AudioType type = GetAudioType(ext);

        using var uwr = new UnityWebRequest(fileUri, UnityWebRequest.kHttpVerbGET);
        var dh = new DownloadHandlerAudioClip(fileUri, type);
        dh.streamAudio = true;
        uwr.downloadHandler = dh;

        await uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"AudioExtensions: failed to load from disk '{fullPath}': {uwr.error}");
            return null;
        }

        return dh.audioClip;
    }

    /// <summary>
    /// Asynchronously downloads an AudioClip + raw bytes from the given URL,
    /// streaming & decoding in the background.
    /// </summary>
    public static async UniTask<(AudioClip clip, byte[] data)> GetAudioClipWithBytesFromUrlAsync(string url)
    {
        string ext = Path.GetExtension(url).ToLower();
        AudioType type = GetAudioType(ext);

        using var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        var dh = new DownloadHandlerAudioClip(url, type);
        dh.streamAudio = true;
        uwr.downloadHandler = dh;

        await uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"AudioExtensions: failed to download '{url}': {uwr.error}");
            return (null, null);
        }

        return (dh.audioClip, uwr.downloadHandler.data);
    }

    private static AudioType GetAudioType(string ext) => ext switch
    {
        ".mp3" or ".mp4" => AudioType.MPEG,
        ".wav" => AudioType.WAV,
        ".ogg" => AudioType.OGGVORBIS,
        _ => AudioType.UNKNOWN,
    };
}
