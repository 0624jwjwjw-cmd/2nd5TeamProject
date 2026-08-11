using System;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    [SerializeField] private float _liveDuration = 20f;

    private float _elapsedTime;
    private bool _isLive;

    private int _lastSecond;
    private int _totalDonation;
    private int _totalSubscribers;

    public bool IsLive => _isLive;
    public float ElapsedTime => _elapsedTime;

    public int TotalDonation => _totalDonation;
    public int TotalSubscribers => _totalSubscribers;

    public event Action OnLiveStarted;
    public event Action OnLiveEnded;
    public event Action<float> OnLiveTimeChanged;
    public event Action<int> OnDonationChanged;
    public event Action<int> OnSubscribersChanged;

    private void Awake()
    {
        _elapsedTime = 0f;
        _lastSecond = 0;
        _totalDonation = 0;
        _totalSubscribers = 0;
    }

    private void Update()
    {
        if (!_isLive)
        {
            return;
        }

        UpdateLiveTimer();
    }

    public void StartLive()
    {
        if (_isLive)
        {
            return;
        }

        _isLive = true;
        _elapsedTime = 0f;
        _lastSecond = 0;

        Debug.Log("라이브 시작!");

        OnLiveStarted?.Invoke();
    }

    public void EndLive()
    {
        if (!_isLive)
        {
            return;
        }

        _isLive = false;

        Debug.Log("라이브 종료");

        OnLiveEnded?.Invoke();
    }

    public void AddFoodReward(DishBase dish)
    {
        if (dish == null)
        {
            return;
        }

        _totalDonation += dish.Donation;
        _totalSubscribers += dish.Subscribers;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);

        Debug.Log(
            $"음식 배치 - {dish.DishName} / " +
            $"후원금 +{dish.Donation} / " +
            $"구독자 +{dish.Subscribers}"
        );
    }

    private void UpdateLiveTimer()
    {
        _elapsedTime += Time.deltaTime;

        int currentSecond = Mathf.FloorToInt(_elapsedTime);

        if (currentSecond > _lastSecond)
        {
            _lastSecond = currentSecond;
            OnLiveTimeChanged?.Invoke(currentSecond);
        }

        if (_elapsedTime >= _liveDuration)
        {
            EndLive();
        }
    }
}