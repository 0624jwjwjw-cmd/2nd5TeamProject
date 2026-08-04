using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveUI : MonoBehaviour
{
    [SerializeField] private LiveManager _liveManager;

    [SerializeField] private Button _startButton;

    [SerializeField] private TMP_Text _liveStatusText;
    [SerializeField] private TMP_Text _liveTimerText;

    private void Awake()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = FormatTime(_liveManager.CurrentTime);

        _startButton.onClick.AddListener(_liveManager.StartLive);
    }

    private void OnEnable()
    {
        _liveManager.OnLiveStarted += ShowLiveStarted;
        _liveManager.OnLiveEnded += ShowLiveEnded;
        _liveManager.OnLiveTimeChanged += UpdateTimer;
    }

    private void OnDisable()
    {
        _liveManager.OnLiveStarted -= ShowLiveStarted;
        _liveManager.OnLiveEnded -= ShowLiveEnded;
        _liveManager.OnLiveTimeChanged -= UpdateTimer;
    }

    private void ShowLiveStarted()
    {
        _liveStatusText.text = "LIVE ON";
    }

    private void ShowLiveEnded()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = "00:00";
    }

    private void UpdateTimer(float currentTime)
    {
        _liveTimerText.text = FormatTime(currentTime);
    }

    private string FormatTime(float time)
    {
        int seconds = Mathf.Max(0, Mathf.CeilToInt(time));

        return $"00:{seconds:00}";
    }
}