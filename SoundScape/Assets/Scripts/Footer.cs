using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Footer : MonoBehaviour
{
    [SerializeField] private Scene scene;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        scene.OnSceneChanged += UpdateVisibility;
    }

    private void OnDisable()
    {
        scene.OnSceneChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(List<SceneItem> sceneItems)
    {
        bool hasItems = sceneItems.Count > 0;
        float alpha = hasItems ? 1F : 0F;

        canvasGroup.FadeTo(alpha, 0.3F, hasItems, hasItems);
    }
}
