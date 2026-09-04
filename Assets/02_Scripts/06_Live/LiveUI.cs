using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveUI : MonoBehaviour
{
    [SerializeField] private LiveManager _liveManager;
    [SerializeField] private FoodArea _foodArea;

    [SerializeField] private Button _startButton;

    [SerializeField] private TMP_Text _liveStatusText;
    [SerializeField] private TMP_Text _liveTimerText;

    private void Awake()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = "00:00";

        _startButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        if (_liveManager == null)
            return;

        _liveManager.OnLiveStarted += ShowLiveStarted;
        _liveManager.OnLiveEnded += ShowLiveEnded;
        _liveManager.OnLiveStopped += ShowLiveStopped;
        _liveManager.OnLiveTimeChanged += UpdateTimer;
    }

    private void OnDisable()
    {
        if (_liveManager != null)
        {
            _liveManager.OnLiveStarted -= ShowLiveStarted;
            _liveManager.OnLiveEnded -= ShowLiveEnded;
            _liveManager.OnLiveStopped -= ShowLiveStopped;
            _liveManager.OnLiveTimeChanged -= UpdateTimer;
        }

        if (_startButton != null)
            _startButton.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_liveManager == null)
            return;

        if (_liveManager.IsLive)
            _liveManager.StopLive();
        else
            _liveManager.StartLive();
    }

    private void ShowLiveStarted()
    {
        _liveStatusText.text = "LIVE ON";
        _startButton.interactable = true;
    }

    private void ShowLiveStopped()
    {
        _liveStatusText.text = "LIVE OFF";

        if (_foodArea != null)
            _foodArea.CheckFoodPlaces();
    }

    private void ShowLiveEnded()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = "00:00";

        if (_foodArea != null)
            _foodArea.CheckFoodPlaces();
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