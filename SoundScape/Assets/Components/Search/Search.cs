using System;
using System.Linq;
using UnityEngine;

public class Search : MonoBehaviour
{
    public enum FilterOption
    {
        AllSounds = 0,
        Premium = 1,
        RecentlyAdded = 2,
        Favorites = 3
    }

    [Tooltip("Drag in your Categories-manager GameObject here")]
    public Categories categories;

    private string _searchTerm = "";
    private FilterOption _currentFilter = FilterOption.AllSounds;

    public void HandleOnSearch(string searchTerm)
    {
        _searchTerm = searchTerm?.ToLowerInvariant() ?? "";
        ApplyFiltering();
    }

    public void HandleOnFilter(int filterIndex)
    {
        _currentFilter = (FilterOption)filterIndex;
        ApplyFiltering();
    }

    private void ApplyFiltering()
    {
        foreach (var cat in categories.Objs)
        {
            // 1) Search‐filter by title
            var matches = cat.Objs
                .Where(c => string.IsNullOrEmpty(_searchTerm)
                            || (c.SoundData.title?.ToLowerInvariant().Contains(_searchTerm) ?? false))
                .ToList();

            // 2) Apply the dropdown filter
            switch (_currentFilter)
            {
                case FilterOption.Premium:
                    matches = matches.Where(c => c.SoundData.isPremium).ToList();
                    break;

                case FilterOption.Favorites:
                    matches = matches.Where(c => c.IsFavorite).ToList();
                    break;

                case FilterOption.RecentlyAdded:
                    // don't drop any—just reorder
                    matches = matches
                        .OrderByDescending(c => c.SoundData.createdAt)
                        .ToList();
                    break;

                case FilterOption.AllSounds:
                default:
                    // no extra filtering
                    break;
            }

            // 3) Hide all, then show & reorder the matches
            foreach (var card in cat.Objs)
                card.gameObject.SetActive(false);

            for (int i = 0; i < matches.Count; i++)
            {
                var card = matches[i];
                card.transform.SetSiblingIndex(i);
                card.gameObject.SetActive(true);
            }

            // 4) Collapse category if empty
            cat.gameObject.SetActive(matches.Count > 0);
        }
    }
}
