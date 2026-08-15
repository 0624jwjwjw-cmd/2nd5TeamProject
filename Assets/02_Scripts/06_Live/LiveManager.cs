using System;
using UnityEngine;


/*
슈퍼 정재운의 후다닥 썰풀기!!
지금 유튜브등급은 음식 먹을때 후원금에만 반응이 와요!
구독자는 스튜디오 업그레이드로 배율이 달라져요!
그래서 이거 2개 구분해야할거에요!
그리고 실험해보면서 느낀건데 돈이랑 구독자가 들어오는게 원가로 들어오더라구여
식빵 원가가 50원이고 후원금 100원 구독자 20명인데
식빵을 먹으면 후원금이랑 구독자가 원가를 기준으로 들어와요
예를 들어서 식빵 3개를 먹으면 150원, 150명이 들어와요
아마 dish.Cost가 아니라 dish.뭐시기 따로 있을거에요
 */

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
        if(!CurrencyManager.Instance.SpendHeart(1))//슈퍼 정재운이 잠시 만진거에용
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

        //int youtubeGrade;

        //switch (dish.ReciepeGrade)
        //{
        //    case "기본":
        //        youtubeGrade = 0;
        //        break;

        //    case "초급":
        //        youtubeGrade = 1;
        //        break;

        //    case "중급":
        //        youtubeGrade = 2;
        //        break;

        //    case "고급":
        //        youtubeGrade = 3;
        //        break;

        //    default:
        //        Debug.LogError(
        //            $"[LiveManager] 알 수 없는 음식 등급입니다: {dish.ReciepeGrade}"
        //        );
        //        return;
        //}
        int youtubeGrade = SubscriberRank.Instance.CurrentRank;//슈퍼 정재운이 잠시 만진거에용


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