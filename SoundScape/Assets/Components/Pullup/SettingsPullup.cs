using UnityEngine;

public class SettingsPullup : Singleton<SettingsPullup>
{
    [SerializeField] private Pullup pullup;
    public void SetActive(bool isOn)
    {
        pullup.SetActivePullup(isOn);
    }
}
