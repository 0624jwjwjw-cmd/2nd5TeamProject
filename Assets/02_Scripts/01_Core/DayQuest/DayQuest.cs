using UnityEngine;
using System;

public class DayQuest : MonoBehaviour, ISaveable
{
    public static DayQuest Instance { get; private set; }

    public event Action<bool> OnQuestResult;

    [Header("Daily Quest Gold")]
    [SerializeField] private int[] targetGold;

    private int currentTargetGold;
    private int earnedGold;

    public int TodayEarnedGold => earnedGold;
    public int CurrentTargetGold => currentTargetGold;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameDateManager.Instance.OnDateChanged += EndQuest;

        RefreshCurrentQuest();
    }

    public void RefreshCurrentQuest()
    {
        int day = GameDateManager.Instance.DateCount
            / GameDateManager.Instance.MaxDateCount;

        if (day >= 0 && day < targetGold.Length)
        {
            currentTargetGold = targetGold[day];
        }
    }

    // 골드가 실제로 증가했을 때 호출
    public void AddEarnedGold(int amount)
    {
        if (amount > 0)
        {
            earnedGold += amount;
        }
    }

    private void OnDestroy()
    {
        if (GameDateManager.Instance != null)
        {
            GameDateManager.Instance.OnDateChanged -= EndQuest;
        }
    }

    private void EndQuest(int dayCount)
    {
        int day = dayCount / GameDateManager.Instance.MaxDateCount;

        if (day >= 0 && day < targetGold.Length)
        {
            currentTargetGold = targetGold[day];
        }

        if (dayCount % GameDateManager.Instance.MaxDateCount == 0)
        {
            CheckQuest(dayCount);
        }
    }

    private void CheckQuest(int dayCount)
    {
        int currentDay = dayCount / GameDateManager.Instance.MaxDateCount;

        // 목표 금액이 설정되지 않은 날짜라면 종료
        if (currentDay - 1 >= targetGold.Length)
        {
            return;
        }

        int target = targetGold[currentDay - 1];

        if (earnedGold >= target)
        {
            QuestSuccess();
        }
        else
        {
            QuestFail();
        }

        // 다음 퀘스트를 위해 초기화
        earnedGold = 0;
    }

    private void QuestSuccess()
    {
        int subscriber = Mathf.RoundToInt(
            CurrencyManager.Instance.Subscriber * 0.2f
        );

        CurrencyManager.Instance.AddSubscriber(subscriber);

        OnQuestResult?.Invoke(true);
    }

    private void QuestFail()
    {
        int subscriber = Mathf.RoundToInt(
            CurrencyManager.Instance.Subscriber * 0.2f
        );

        CurrencyManager.Instance.AddSubscriber(-subscriber);

        OnQuestResult?.Invoke(false);
    }

    public void Save(SaveData data)
    {
        data.questEarnedGold = earnedGold;
    }

    public void Load(SaveData data)
    {
        earnedGold = data.questEarnedGold;
    }

    // 게임 완전 초기화용
    public void ResetQuestProgress()
    {
        earnedGold = 0;
        RefreshCurrentQuest();
    }
}