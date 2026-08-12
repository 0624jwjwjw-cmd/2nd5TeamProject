using System;
using UnityEngine;

public class SubscriberRank : MonoBehaviour
{
    public event Action<int> OnRankChanged;
    private int currentRank = 1;
    public int CurrentRank => currentRank;

    private void Start()
    {
        CurrencyManager.Instance.OnRevenueChanged += CheckRank;

        CheckRank();
    }

    private void OnDestroy()
    {
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

        if (subscriber >= 500000)
            return 5;

        if (subscriber >= 150000)
            return 4;

        if (subscriber >= 30000)
            return 3;

        if (subscriber >= 5000)
            return 2;

        return 1;
    }
    public int GetNextRankRequirement()
    {
        switch (CurrentRank)
        {
            case 1:
                return 5000;
            case 2:
                return 30000;
            case 3:
                return 150000;
            case 4:
                return 500000;
            default:
                return -1;
        }
    }
}
