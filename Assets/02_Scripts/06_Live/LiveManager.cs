using System;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance { get; private set; }

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
    public event Action OnLiveStopped;
    public event Action OnLiveEnded;
    public event Action<float> OnLiveTimeChanged;
    public event Action<int> OnDonationChanged;
    public event Action<int> OnSubscribersChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _elapsedTime = 0f;
        _lastSecond = 0;
        _totalDonation = 0;
        _totalSubscribers = 0;
    }

    private void Update()
    {
        if (!_isLive)
            return;

        _elapsedTime += Time.deltaTime;

        int currentSecond = Mathf.FloorToInt(_elapsedTime);

        if (currentSecond > _lastSecond)
        {
            _lastSecond = currentSecond;
            OnLiveTimeChanged?.Invoke(currentSecond);
        }

        if (_elapsedTime >= _liveDuration)
            EndLive();
    }

    public void StartLive()
    {
        if (_isLive)
            return;

        if (!CurrencyManager.Instance.SpendHeart(1))
        {
            Debug.Log("하트가 부족합니다.");
            return;
        }

        _isLive = true;
        _elapsedTime = 0f;
        _lastSecond = 0;

        _totalDonation = 0;
        _totalSubscribers = 0;

        Debug.Log("라이브 시작!");

        OnLiveTimeChanged?.Invoke(0);
        OnLiveStarted?.Invoke();
    }

    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        Debug.Log("라이브 중단");

        OnLiveStopped?.Invoke();
    }

    public void EndLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        Debug.Log(
            $"라이브 종료 / 후원금: {_totalDonation} / 구독자: {_totalSubscribers}"
        );

        OnLiveEnded?.Invoke();
    }

    public void EatFood(string itemId)
    {
        if (!_isLive)
            return;

        if (string.IsNullOrEmpty(itemId))
            return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager가 없습니다.");
            return;
        }

        if (GameDataRepository.Instance == null)
        {
            Debug.LogError("[LiveManager] GameDataRepository가 없습니다.");
            return;
        }

        DishData dishData;

        if (GameDataRepository.Instance.TryGetDish(itemId, out dishData))
        {
        }
        else if (GameDataRepository.Instance.TryGetSpecialDish(
            itemId,
            out dishData))
        {
        }
        else
        {
            Debug.LogError(
                $"[LiveManager] 음식 데이터를 찾을 수 없습니다. ID: {itemId}"
            );

            return;
        }

        int youtubeGrade = SubscriberRank.Instance.CurrentRank;

        int beforeGold = CurrencyManager.Instance.Gold;
        int beforeSubscriber = CurrencyManager.Instance.Subscriber;

        CalculateGold.GetDonation(
            dishData.Cost,
            youtubeGrade
        );

        CalculateSubscriber.GetDonation(
            dishData.Cost,
            youtubeGrade
        );

        int addedGold =
            CurrencyManager.Instance.Gold - beforeGold;

        int addedSubscriber =
            CurrencyManager.Instance.Subscriber - beforeSubscriber;

        _totalDonation += addedGold;
        _totalSubscribers += addedSubscriber;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);

        Debug.Log(
            $"음식 섭취 - {dishData.DishName} / " +
            $"등급: {dishData.ReciepeGrade} / " +
            $"후원금 +{addedGold} / " +
            $"구독자 +{addedSubscriber}"
        );
    }
}