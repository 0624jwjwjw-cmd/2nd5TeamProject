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

    // 라이브 동안 먹은 음식의 총 원가
    private int _totalFoodCost;

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
        _totalFoodCost = 0;
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

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager가 없습니다.");
            return;
        }

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
        _totalFoodCost = 0;

        Debug.Log("라이브 시작!");

        OnLiveTimeChanged?.Invoke(0);
        OnLiveStarted?.Invoke();
    }

    // 사용자가 방송을 중단했을 때
    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        Debug.Log("라이브 중단");

        OnLiveStopped?.Invoke();
    }

    // 라이브 시간이 끝났을 때
    public void EndLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        CalculateLiveReward();

        if (GameDateManager.Instance != null)
            GameDateManager.Instance.AddDateCount();

        Debug.Log(
            $"라이브 종료 / 후원금: {_totalDonation} / " +
            $"구독자: {_totalSubscribers}"
        );

        OnLiveEnded?.Invoke();
    }

    // 음식의 ItemId를 받아 원가를 누적
    public void EatFood(string itemId)
    {
        if (!_isLive)
            return;

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        if (GameDataRepository.Instance == null)
        {
            Debug.LogError("[LiveManager] GameDataRepository가 없습니다.");
            return;
        }

        DishData dishData;

        if (!GameDataRepository.Instance.TryGetDish(
            itemId,
            out dishData))
        {
            if (!GameDataRepository.Instance.TryGetSpecialDish(
                itemId,
                out dishData))
            {
                Debug.LogWarning(
                    $"[LiveManager] 음식 데이터를 찾을 수 없습니다. ID: {itemId}"
                );

                return;
            }
        }

        _totalFoodCost += dishData.Cost;

        Debug.Log(
            $"음식 섭취 - {dishData.DishName} / " +
            $"원가: {dishData.Cost} / " +
            $"누적 원가: {_totalFoodCost}"
        );
    }

    // 라이브 종료 시 후원금과 구독자를 계산
    private void CalculateLiveReward()
    {
        if (_totalFoodCost <= 0)
            return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager가 없습니다.");
            return;
        }

        if (SubscriberRank.Instance == null)
        {
            Debug.LogError("[LiveManager] SubscriberRank가 없습니다.");
            return;
        }

        int youtubeGrade = SubscriberRank.Instance.CurrentRank;

        int beforeGold = CurrencyManager.Instance.Gold;
        int beforeSubscriber = CurrencyManager.Instance.Subscriber;

        CalculateGold.GetDonation(
            _totalFoodCost,
            youtubeGrade
        );

        CalculateSubscriber.GetDonation(
            _totalFoodCost,
            youtubeGrade
        );

        _totalDonation =
            CurrencyManager.Instance.Gold - beforeGold;

        _totalSubscribers =
            CurrencyManager.Instance.Subscriber - beforeSubscriber;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);
    }
}