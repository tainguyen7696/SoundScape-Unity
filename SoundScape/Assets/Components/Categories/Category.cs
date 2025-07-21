using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class CategoryData
{
    public string title;
    public List<Sounds> soundDatas;
}


public class Category : Factory<SoundCard>
{
    public string Title
    {
        get => title.text;
        set => title.text = value;
    }
    [Space]
    [SerializeField] private TextMeshProUGUI title;

    public SoundCard CreateOrGet(SoundData data)
    {
        var obj = Objs.FirstOrDefault(x => x.SoundData.title == data.title);
        if (obj == null)
        {
            obj = Create();
            obj.Download(data);
        }
        return obj;
    }
}
