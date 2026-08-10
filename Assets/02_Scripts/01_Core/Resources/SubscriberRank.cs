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


        if (subscriber >= 30000)
            return 4;

        if (subscriber >= 20000)
            return 3;

        if (subscriber >= 10000)
            return 2;

        return 1;
    }
}
