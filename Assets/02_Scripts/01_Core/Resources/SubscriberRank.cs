using System;
using UnityEngine;

public class SubscriberRank : MonoBehaviour
{
    public static SubscriberRank Instance { get; private set; }

    public event Action<int> OnRankChanged;

    private int currentRank = 1;
    public int CurrentRank => currentRank;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrencyManager.Instance.OnRevenueChanged += CheckRank;

        currentRank = CalculateRank();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnRevenueChanged -= CheckRank;
    }

    private void CheckRank()
    {
        int newRank = CalculateRank();

        if (newRank != currentRank)
        {
            currentRank = newRank;
            OnRankChanged?.Invoke(currentRank);
        }
    }

    private int CalculateRank()
    {
        int subscriber = CurrencyManager.Instance.Subscriber;

        int rank = 1;

        for (int i = 1; i <= GradeDatabase.Instance.GradeCount; i++)
        {
            GradeData gradeData = GradeDatabase.Instance.GetGrade(i);

            if (subscriber >= gradeData.RequiredSubscribers)
            {
                rank = i;
            }
        }

        return rank;
    }

    public int GetNextRankRequirement()
    {
        if (currentRank >= GradeDatabase.Instance.GradeCount)
            return -1;

        return GradeDatabase.Instance
            .GetGrade(currentRank + 1)
            .RequiredSubscribers;
    }

    public GradeData GetCurrentGradeData()
    {
        return GradeDatabase.Instance.GetGrade(currentRank);
    }
}