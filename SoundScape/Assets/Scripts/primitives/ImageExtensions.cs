// ImageExtensions.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public static class ImageExtensions
{
    /// <summary>
    /// Extension method—calls a coroutine under the hood to fetch the texture,
    /// turn it into a sprite, preserve its aspect, fit it to the Image’s width,
    /// then resize the RectTransform’s height to match.
    /// </summary>
    public static void LoadImageFromURL(this Image image, string url)
    {
        // Kick off the coroutine on our runner
        CoroutineRunner.Instance.StartCoroutine(LoadImageRoutine(image, url));
    }

    private static IEnumerator LoadImageRoutine(Image image, string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (uwr.result != UnityWebRequest.Result.Success)
#else
            if (uwr.isNetworkError || uwr.isHttpError)
#endif
            {
                Debug.LogError($"Failed to download image at {url}: {uwr.error}");
                yield break;
            }

            // 1) Get the downloaded texture
            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);

            // 2) Create a sprite centered on pivot
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            // 3) Assign it and preserve aspect
            image.overrideSprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;

            // 4) Fit to full width, then recalc height to maintain original aspect
            RectTransform rt = image.rectTransform;
            float targetWidth = rt.rect.width;
            float newHeight = targetWidth * ((float)tex.height / tex.width);
            rt.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                newHeight
            );
        }
    }

    /// <summary>
    /// Downloads an image at the given URL, converts it to a Sprite,
    /// and returns it via the onLoaded callback.
    /// </summary>
    /// <param name="url">The URL of the image to download.</param>
    /// <param name="onLoaded">Callback invoked with the created Sprite (or null on failure).</param>
    public static void LoadSpriteFromURL(string url, Action<Sprite> onLoaded)
    {
        CoroutineRunner.Instance.StartCoroutine(LoadSpriteRoutine(url, onLoaded));
    }

    private static IEnumerator LoadSpriteRoutine(string url, Action<Sprite> onLoaded)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (uwr.result != UnityWebRequest.Result.Success)
#else
            if (uwr.isNetworkError || uwr.isHttpError)
#endif
            {
                Debug.LogError($"Failed to download image at {url}: {uwr.error}");
                onLoaded?.Invoke(null);
                yield break;
            }

            // Grab the texture and create a sprite
            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            onLoaded?.Invoke(sprite);
        }
    }
}
