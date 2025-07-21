using UnityEngine;

public class Footer : MonoBehaviour
{
    [SerializeField] private Scene scene;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        scene.OnSceneChanged += UpdateVisibility;
        UpdateVisibility(scene.Count);
    }

    private void OnDisable()
    {
        scene.OnSceneChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(int count)
    {
        bool hasItems = count > 0;
        float alpha = hasItems ? 1F : 0F;

        canvasGroup.FadeTo(alpha, 0.3F, hasItems, hasItems);
    }
}
