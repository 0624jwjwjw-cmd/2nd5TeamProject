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
        if(CurrencyManager.Instance.SpendHeart(1))//슈퍼 정재운이 잠시 만진거에용
        {
            Debug.Log("하트 1개 사용");
        }
        else
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

    // 사용자가 방송 중단 버튼을 눌렀을 때
    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        Debug.Log("라이브 중단");

        OnLiveStopped?.Invoke();
    }

    // 20초가 끝났을 때
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

    public void EatFood(DishBase dish)
    {
        if (!_isLive || dish == null)
            return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager가 없습니다.");
            return;
        }

        int youtubeGrade;

        switch (dish.ReciepeGrade)
        {
            case "기본":
                youtubeGrade = 0;
                break;

            case "초급":
                youtubeGrade = 1;
                break;

            case "중급":
                youtubeGrade = 2;
                break;

            case "고급":
                youtubeGrade = 3;
                break;

            default:
                Debug.LogError(
                    $"[LiveManager] 알 수 없는 음식 등급입니다: {dish.ReciepeGrade}"
                );
                return;
        }

        int beforeGold = CurrencyManager.Instance.Gold;
        int beforeSubscriber = CurrencyManager.Instance.Subscriber;

        CalculateGold.GetDonation(
            dish.Cost,
            youtubeGrade
        );

        CalculateSubscriber.GetDonation(
            dish.Cost,
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
            $"음식 섭취 - {dish.DishName} / " +
            $"등급: {dish.ReciepeGrade} / " +
            $"후원금 +{addedGold} / " +
            $"구독자 +{addedSubscriber}"
        );
    }
}