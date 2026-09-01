using UnityEngine;
using System;
public class DayQuest : MonoBehaviour
{
    public static DayQuest Instance { get; private set; }
    public event Action<bool> OnQuestResult;

    [Header("Daily Quest Gold")]
    [SerializeField] private int[] targetGold;

    private int startGold = 5000;
    private int currentTargetGold;
    public int TodayEarnedGold => CurrencyManager.Instance.Gold - startGold;
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
        startGold = CurrencyManager.Instance.Gold;
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
        int currentDay = dayCount / 5;

        // 목표 금액이 설정되지 않은 날짜라면 종료
        if (currentDay - 1 >= targetGold.Length)
        {
            Debug.Log("퀘없음");
            return;
        }
        int currentGold = CurrencyManager.Instance.Gold;
        int earnedGold = currentGold - startGold;
        int target = targetGold[currentDay - 1];
        if (earnedGold >= target)
        {
            QuestSuccess();
        }
        else
        {
            QuestFail();
        }

        startGold = currentGold;
    }
    private void QuestSuccess()
    {
        Debug.Log("퀘스트 성공!");

        int subscriber =Mathf.RoundToInt(CurrencyManager.Instance.Subscriber * 0.2f);
        CurrencyManager.Instance.AddSubscriber(subscriber);
        OnQuestResult?.Invoke(true);
    }

    private void QuestFail()
    {
        Debug.Log("퀘스트 실패!");

        int subscriber = Mathf.RoundToInt(CurrencyManager.Instance.Subscriber * 0.2f);
        CurrencyManager.Instance.AddSubscriber(-subscriber);
        OnQuestResult?.Invoke(false);
    }

    public void ResetQuestProgress()
    {
        startGold = CurrencyManager.Instance.Gold;
        RefreshCurrentQuest();
    }
}
