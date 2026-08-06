using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveUI : MonoBehaviour
{
    [SerializeField] private LiveManager _liveManager;

    [SerializeField] private Button _startButton;

    [SerializeField] private TMP_Text _liveStatusText;
    [SerializeField] private TMP_Text _liveTimerText;

    [SerializeField] private TMP_Text _viewerText;
    [SerializeField] private TMP_Text _subscriberText;
    [SerializeField] private TMP_Text _donationText;

    private void Awake()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = FormatTime(_liveManager.ElapsedTime);
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
    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(_liveManager.StartLive);
    }
    private void ShowLiveStarted()
    {
        _liveStatusText.text = "LIVE ON";
    }

    private void ShowLiveEnded()
    {
        _liveStatusText.text = "LIVE OFF";
    }

    private void UpdateTimer(float currentSecond)
    {
        _liveTimerText.text = FormatTime(currentSecond);
    }

    private string FormatTime(float currentSecond)
    {
        return $"00:{(int)currentSecond:00}";
    }
}