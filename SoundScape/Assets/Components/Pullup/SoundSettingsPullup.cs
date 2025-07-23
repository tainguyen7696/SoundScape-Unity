using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsPullup : Singleton<SoundSettingsPullup>
{
    [SerializeField] private Pullup pullup;
    [SerializeField] private Slider volumeSlider, warmthSlider;

    private SceneItem sceneItem;
    public void Download(SceneItem sceneItem)
    {
        this.sceneItem = sceneItem;
        pullup.Title = sceneItem.SoundData.title;
        volumeSlider.value = sceneItem.SoundData.volume;
        warmthSlider.value = sceneItem.SoundData.warmth;
    }

    public void SetActive(bool isOn)
    {
        pullup.SetActivePullup(isOn);
    }

    public void OnVolumeSliderChange(float value)
    {
        sceneItem.SoundData.volume = value;
        SoundSceneController.Instance.SetLayerVolume(sceneItem.LayerIndex, value);
        SoundDataManager.Instance.SaveAllJsonCache();
    }
    public void OnWarmthSliderChange(float value)
    {
        sceneItem.SoundData.warmth = value;
        SoundSceneController.Instance.SetLayerWarmth(sceneItem.LayerIndex, value);
        SoundDataManager.Instance.SaveAllJsonCache();
    }
}
