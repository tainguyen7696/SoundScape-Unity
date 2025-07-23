// Assets/Scripts/SoundData.cs
using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class SoundData
{
    [JsonProperty("title")] public string title { get; set; }

    [JsonProperty("audioUrl")] public string audioUrl { get; set; }

    [JsonProperty("backgroundImageUrl")] public string backgroundImageUrl { get; set; }

    [JsonProperty("backgroundImagePath")] public string backgroundImagePath { get; set; }

    [JsonProperty("audioClipPath")] public string audioClipPath { get; set; }

    [JsonIgnore] public AudioClip audioClip { get; set; }

    [JsonIgnore] public Sprite backgroundImage { get; set; }

    [JsonProperty("category")] public string category { get; set; }

    [JsonProperty("isPremium")] public bool isPremium { get; set; }

    [JsonProperty("createdAt")] public DateTime createdAt { get; set; }

    [JsonProperty("volume")] public float volume { get; set; }
    [JsonProperty("warmth")] public float warmth { get; set; }
}
