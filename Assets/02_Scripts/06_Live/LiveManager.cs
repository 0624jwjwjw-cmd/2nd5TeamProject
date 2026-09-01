using System;
using System.Collections;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance { get; private set; }

    [Header("Live")]
    [SerializeField] private float _liveDuration = 20f;

    [Header("Mini Game")]
    [SerializeField] private MiniGameStarter _miniGameStarter;
    [SerializeField] private float _miniGameStartDelay = 3f;

    private float _elapsedTime;
    private bool _isLive;
    private int _lastSecond;

    private int _totalDonation;
    private int _totalSubscribers;
    private int _totalFoodCost;

    private Coroutine _miniGameCoroutine;

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

        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);
        SoundManager.Instance?.PlayBGM(BGMType.Studio);

        OnLiveTimeChanged?.Invoke(0);
        OnLiveStarted?.Invoke();

        StartMiniGameTimer();

        Debug.Log("라이브 시작!");
    }

    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        StopMiniGameTimer();

        SoundManager.Instance?.PlayBGM(BGMType.Normal);

        OnLiveStopped?.Invoke();

        Debug.Log("라이브 중단");
    }

    public void EndLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        StopMiniGameTimer();

        CalculateLiveReward();

        GameDateManager.Instance.AddDateCount();

        SoundManager.Instance?.PlayBGM(BGMType.Normal);

        OnLiveEnded?.Invoke();

        Debug.Log(
            $"라이브 종료 / 후원금: {_totalDonation} / " +
            $"구독자: {_totalSubscribers}"
        );
    }

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

        if (!GameDataRepository.Instance.TryGetDish(itemId, out dishData))
        {
            if (!GameDataRepository.Instance.TryGetSpecialDish(
                itemId,
                out dishData))
            {
                Debug.LogWarning(
                    $"[LiveManager] 음식을 찾을 수 없습니다. ID: {itemId}"
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

    private void StartMiniGameTimer()
    {
        StopMiniGameTimer();

        if (_miniGameStarter == null)
        {
            Debug.LogWarning(
                "[LiveManager] MiniGameStarter가 연결되지 않았습니다."
            );
            return;
        }

        _miniGameCoroutine = StartCoroutine(StartMiniGameAfterDelay());
    }

    private void StopMiniGameTimer()
    {
        if (_miniGameCoroutine == null)
            return;

        StopCoroutine(_miniGameCoroutine);
        _miniGameCoroutine = null;
    }

    private IEnumerator StartMiniGameAfterDelay()
    {
        yield return new WaitForSeconds(_miniGameStartDelay);

        _miniGameCoroutine = null;

        if (!_isLive)
            yield break;

        _miniGameStarter.StartMiniGame();
    }

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

        if (StudioUpgradeManager.Instance == null)
        {
            Debug.LogError("[LiveManager] StudioUpgradeManager가 없습니다.");
            return;
        }

        if (StudioUpgradeManager.Instance.CurrentData == null)
        {
            Debug.LogError(
                "[LiveManager] StudioUpgradeManager의 CurrentData가 없습니다."
            );
            return;
        }

        int beforeGold = CurrencyManager.Instance.Gold;
        int beforeSubscriber = CurrencyManager.Instance.Subscriber;

        // 골드(후원금)는 기존 유튜브 랭크 배율 사용
        int youtubeGrade = SubscriberRank.Instance.CurrentRank;

        CalculateGold.GetDonation(
            _totalFoodCost,
            youtubeGrade
        );

        // 구독자는 스튜디오 업그레이드 배율 사용
        float subscriberBonus =
            StudioUpgradeManager.Instance.CurrentData.SubscriberBonus;

        int subscribers = Mathf.RoundToInt(
            _totalFoodCost * subscriberBonus
        );

        CurrencyManager.Instance.AddSubscriber(subscribers);

        _totalDonation =
            CurrencyManager.Instance.Gold - beforeGold;

        _totalSubscribers =
            CurrencyManager.Instance.Subscriber - beforeSubscriber;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);

        Debug.Log(
            $"라이브 보상 계산 / " +
            $"후원금: {_totalDonation} / " +
            $"구독자: {_totalSubscribers} / " +
            $"스튜디오 배율: {subscriberBonus}"
        );
    }
}