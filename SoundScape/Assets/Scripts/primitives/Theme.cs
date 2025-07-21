using UnityEngine;

/// <summary>
/// ScriptableObject asset to hold theme colors for the application.
/// Create instances via Assets → Create → App → Theme.
/// </summary>
[CreateAssetMenu(fileName = "Theme", menuName = "App/Theme", order = 0)]
public class ThemeSO : ScriptableObject
{
    [Header("Colors")]
    public Color background = new Color32(0xF9, 0xF9, 0xF9, 0xFF);
    public Color background2 = Color.white;
    public Color background3 = Color.white;
    public Color icon = Color.white;
    public Color cardBackground = new Color32(0xE0, 0xE0, 0xE0, 0xFF);
    public Color text = Color.black;
    public Color textDim = new Color32(0xCC, 0xCC, 0xCC, 0xFF);
    public Color primary = new Color32(0x00, 0x7B, 0xFF, 0xFF);
    public Color overlay = new Color(0f, 0f, 0f, 0.5f);
}
