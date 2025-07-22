using UnityEngine;
using Supabase;
using Supabase.Gotrue;
using Newtonsoft.Json;
using Supabase.Gotrue.Interfaces;

public class SupabaseManager : Singleton<SupabaseManager>
{
    [Header("Supabase Settings")]
    [Tooltip("Your Supabase project URL (e.g., https://xyz.supabase.co)")]
    public string SupabaseUrl = "https://biipaaggidcraekofbuq.supabase.co";
    [Tooltip("Your Supabase anon/public key")]
    public string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJpaXBhYWdnaWRjcmFla29mYnVxIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTA0OTMwOTUsImV4cCI6MjA2NjA2OTA5NX0.WOI9tQ_S7C-uvWBK3eIAEe5y6lPNzYVVhymHeSPAZbs";

    public static Supabase.Client Client { get; private set; }

    public override async void Awake()
    {
        base.Awake();
        var options = new SupabaseOptions
        {
            AutoRefreshToken    = true,
            AutoConnectRealtime = true,
            SessionHandler      = new PlayerPrefsSessionHandler() 
        };

        Client = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
        await Client.InitializeAsync();

        Debug.Log("✅ Supabase client initialized");

        if (Client.Auth.CurrentUser != null)
            Debug.Log($"Welcome back {Client.Auth.CurrentUser.Email}");
    }
}

// Example session handler storing Session in Unity’s PlayerPrefs
public class PlayerPrefsSessionHandler : IGotrueSessionPersistence<Session>
{
    private const string SessionKey = "supabase_session";

    public void SaveSession(Session session)
    {
        var json = JsonConvert.SerializeObject(session);
        PlayerPrefs.SetString(SessionKey, json);
        PlayerPrefs.Save();
    }

    public Session LoadSession()
    {
        if (!PlayerPrefs.HasKey(SessionKey))
            return null;
        var json = PlayerPrefs.GetString(SessionKey);
        return JsonConvert.DeserializeObject<Session>(json);
    }

    public void DestroySession()
    {
        PlayerPrefs.DeleteKey(SessionKey);
    }
}
