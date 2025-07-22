using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerSettingsPullup : Singleton<TimerSettingsPullup>
{
    [Header("UI References")]
    [SerializeField] private Pullup pullUp;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI subHeaderText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Slider slider;

    // Internal state
    private int minutes;
    private float remainingSeconds;
    private bool isRunning = false;

    public event Action<TimeSpan> OnTimerTick;
    public event Action OnTimerComplete;

    public override void Awake()
    {
        base.Awake();

        // Hide on start
        pullUp.SetActiveBlocker(false);
        pullUp.SetActivePullup(false);
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingSeconds -= Time.deltaTime;
        if (remainingSeconds <= 0f)
        {
            remainingSeconds = 0f;
            OnTimerComplete?.Invoke();
            isRunning = false;
        }

        Debug.Log(remainingSeconds);
        // broadcast time
        OnTimerTick?.Invoke(TimeSpan.FromSeconds(remainingSeconds));

        // update UI to show HH:MM:SS countdown
        UpdateCountdownUI();
    }

    public void SetActive(bool isOn)
    {
        ResetTimer();
        isRunning = true;
        pullUp.SetActivePullup(isOn);
    }

    public void OnSliderValueChanged(float value)
    {
        ResetTimer();
        isRunning = true;
        minutes = Mathf.RoundToInt(value);
        UpdateCountdownUI();
    }

    private void UpdateCountdownUI()
    {
        int totalSec = Mathf.CeilToInt(remainingSeconds);
        int hrs = totalSec / 3600;
        int mins = (totalSec % 3600) / 60;
        int secs = totalSec % 60;

        subHeaderText.text =
            $"{hrs}h {mins:00}m {secs:00}s";

        DateTime endTime = DateTime.Now.AddSeconds(remainingSeconds);
        endText.text = $"Ends at: {endTime.ToShortTimeString()}";
    }

    private void ResetTimer()
    {
        minutes = (int)slider.value;
        remainingSeconds = minutes * 60f;
    }
}
