using UnityEngine;

public static class CalculateGold
{
    private static readonly float[] youtubeMultiplier =
    {
        1f,
        1.15f,
        1.3f,
        1.5f,
        1.7f
    };

    public static void GetDonation(int foodCost, int youtubeGrade)
    {
        int donation = Mathf.RoundToInt(
            foodCost * youtubeMultiplier[youtubeGrade]);

        CurrencyManager.Instance.AddGold(donation);
    }
}
