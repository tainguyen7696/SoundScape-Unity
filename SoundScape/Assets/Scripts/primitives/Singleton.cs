using UnityEngine;

/// <summary>
/// Generic singleton base for any MonoBehaviour.
/// Inherit from this to make your own singleton component:
/// 
/// public class GameManager : Singleton<GameManager> { … }
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// Accessor for the singleton instance.
    /// Creates one at runtime if none exists in the scene.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try find existing
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    // Create new GameObject with T
                    var singletonGO = new GameObject(typeof(T).Name);
                    _instance = singletonGO.AddComponent<T>();
                }
                // Persist across scenes
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    /// <summary>
    /// On Awake, enforce the singleton property.
    /// </summary>
    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
