using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveUI : MonoBehaviour
{
    [SerializeField] private LiveManager _liveManager;

    [SerializeField] private Button _startButton;
    [SerializeField] private TMP_Text _startButtonText;

    [SerializeField] private TMP_Text _liveStatusText;
    [SerializeField] private TMP_Text _liveTimerText;

    [SerializeField] private TMP_Text _viewerText;
    [SerializeField] private TMP_Text _subscriberText;
    [SerializeField] private TMP_Text _donationText;

    private void Awake()
    {
        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = "00:00";
        _startButtonText.text = "방송 시작";
        _donationText.text = "후원금 0";
        _subscriberText.text = "구독자 0";

        _startButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        _liveManager.OnLiveStarted += ShowLiveStarted;
        _liveManager.OnLiveEnded += ShowLiveEnded;
        _liveManager.OnLiveTimeChanged += UpdateTimer;
        _liveManager.OnDonationChanged += UpdateDonation;
        _liveManager.OnSubscribersChanged += UpdateSubscribers;
    }

    private void OnDisable()
    {
        if (_liveManager != null)
        {
            _liveManager.OnLiveStarted -= ShowLiveStarted;
            _liveManager.OnLiveEnded -= ShowLiveEnded;
            _liveManager.OnLiveTimeChanged -= UpdateTimer;
            _liveManager.OnDonationChanged -= UpdateDonation;
            _liveManager.OnSubscribersChanged -= UpdateSubscribers;
        }

        if (_startButton != null)
        {
            _startButton.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (_liveManager == null)
        {
            return;
        }

        if (_liveManager.IsLive)
        {
            _liveManager.EndLive();
        }
        else
        {
            _liveManager.StartLive();
        }
    }

    private void ShowLiveStarted()
    {
        _liveStatusText.text = "LIVE ON";
        _startButtonText.text = "방송 중단";
        _startButton.interactable = true;
    }

    private void ShowLiveEnded()
    {
        _liveStatusText.text = "LIVE OFF";
        _startButtonText.text = "방송 시작";
        _liveTimerText.text = "00:00";
    }

    private void UpdateTimer(float currentSecond)
    {
        _liveTimerText.text = FormatTime(currentSecond);
    }

    private string FormatTime(float currentSecond)
    {
        return $"00:{(int)currentSecond:00}";
    }

    private void UpdateDonation(int donation)
    {
        _donationText.text = $"후원금 {donation}";
    }

    private void UpdateSubscribers(int subscribers)
    {
        _subscriberText.text = $"구독자 {subscribers}";
    }
}