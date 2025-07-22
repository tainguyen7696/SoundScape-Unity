// Assets/Scripts/AudioExtensions.cs
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class AudioExtensions
{
    /// <summary>
    /// Synchronously downloads an AudioClip and its raw bytes from the given URL (HTTP or file://).
    /// Blocks the main thread until the download completes—use with caution.
    /// </summary>
    /// <param name="url">HTTP or file:// URL to fetch.</param>
    /// <returns>
    /// A ValueTuple where
    ///  - .clip is the downloaded AudioClip (or null on error)
    ///  - .data is the raw byte[] that was downloaded (or null on error)
    /// </returns>
    public static (AudioClip clip, byte[] data) GetAudioClipWithBytesFromUrl(string url)
    {
        // Determine format by extension
        string ext = Path.GetExtension(url).ToLower();
        AudioType type = GetAudioType(ext);
        byte[] bytes = null;
        AudioClip clip = null;

        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, type))
        {
            var req = uwr.SendWebRequest();
            // BLOCKING wait
            while (!req.isDone) { }

#if UNITY_2020_1_OR_NEWER
            if (uwr.result != UnityWebRequest.Result.Success)
#else
            if (uwr.isNetworkError || uwr.isHttpError)
#endif
            {
                Debug.LogError($"AudioExtensions: failed to load clip from {url}: {uwr.error}");
                return (null, null);
            }

            // grab raw bytes
            bytes = uwr.downloadHandler.data;
            // convert to AudioClip
            clip = DownloadHandlerAudioClip.GetContent(uwr);
        }

        return (clip, bytes);
    }

    private static AudioType GetAudioType(string ext) => ext switch
    {
        ".mp3" or ".mp4" => AudioType.MPEG,
        ".wav" => AudioType.WAV,
        ".ogg" => AudioType.OGGVORBIS,
        _ => AudioType.UNKNOWN,
    };
}
