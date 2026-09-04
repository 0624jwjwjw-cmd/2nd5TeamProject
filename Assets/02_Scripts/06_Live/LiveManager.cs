using System;
using System.Collections;
using System.Net;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance { get; private set; }

    [Header("Live")]
    [SerializeField] private float _liveDuration = 20f;

    [Header("Character")]
    [SerializeField] private LiveCharacterTween characterTween;

    [Header("Mini Game")]
    [SerializeField] private MiniGameStarter _miniGameStarter;
    [SerializeField] private float _miniGameStartDelay = 3f;

    private float _elapsedTime;
    private bool _isLive;
    private int _lastSecond;

    private int _totalDonation;
    private int _totalSubscribers;

    // 기본
    private int _baseDonation;
    private int _baseSubscribers;

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

    // 음식 섭취 시 이름 전달
    public event Action<string> OnFoodEaten;

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
            return;

        if (!CurrencyManager.Instance.SpendHeart(1))
            return;

        _isLive = true;
        _elapsedTime = 0f;
        _lastSecond = 0;

        _totalDonation = 0;
        _totalSubscribers = 0;

        _baseDonation = 0;
        _baseSubscribers = 0;

        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);
        SoundManager.Instance?.PlayBGM(BGMType.Studio);

        OnLiveTimeChanged?.Invoke(0);
        OnLiveStarted?.Invoke();

        StartMiniGameTimer();
    }

    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        StopMiniGameTimer();

        SoundManager.Instance?.PlayBGM(BGMType.Normal);

        OnLiveStopped?.Invoke();
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
    }

    public void EatFood(string itemId)
    {
        if (!_isLive)
            return;

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        if (GameDataRepository.Instance == null)
            return;

        DishData dishData;

        // 일반 음식 확인
        if (!GameDataRepository.Instance.TryGetDish(
                itemId,
                out dishData))
        {
            // 특별 음식 확인
            if (!GameDataRepository.Instance.TryGetSpecialDish(
                    itemId,
                    out dishData))
            {
                return;
            }
        }

        // 음식 후원금, 구독자 누적
        _baseDonation += dishData.Donation;
        _baseSubscribers += dishData.Subscribers;

        _miniGameStarter.AddPoint(
            dishData.Donation,
            dishData.Subscribers
        );

        characterTween?.PlayEatReaction();

        SoundManager.Instance?.PlaySFX(SFXType.Eat);

        // 음식 정보를 외부 시스템에 전달
        // LiveChat이 이벤트를 받아 AI 채팅 생성
        OnFoodEaten?.Invoke(dishData.DishName);
    }

    private void StartMiniGameTimer()
    {
        StopMiniGameTimer();

        if (_miniGameStarter == null)
            return;

        _miniGameCoroutine =
            StartCoroutine(StartMiniGameAfterDelay());
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
        if (_baseDonation <= 0 && _baseSubscribers <= 0)
            return;

        if (CurrencyManager.Instance == null)
            return;

        if (SubscriberRank.Instance == null)
            return;

        if (StudioUpgradeManager.Instance == null)
            return;

        if (StudioUpgradeManager.Instance.CurrentData == null)
            return;

        int beforeGold =
            CurrencyManager.Instance.Gold;

        int beforeSubscriber =
            CurrencyManager.Instance.Subscriber;

        int youtubeGrade =
            SubscriberRank.Instance.CurrentRank;

        CalculateGold.GetDonation(
            _baseDonation,
            youtubeGrade
        );

        float subscriberBonus =
            StudioUpgradeManager.Instance.CurrentData.SubscriberBonus;

        int finalSubscribers =
            Mathf.RoundToInt(
                _baseSubscribers * subscriberBonus
            );

        CurrencyManager.Instance.AddSubscriber(
            finalSubscribers
        );

        // 실제 획득량 계산
        _totalDonation =
            CurrencyManager.Instance.Gold - beforeGold;

        _totalSubscribers =
            CurrencyManager.Instance.Subscriber - beforeSubscriber;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);
    }
}