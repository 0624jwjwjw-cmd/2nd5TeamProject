using UnityEngine;

public static class CalculateGold
{
    public static void GetDonation(int foodCost, int youtubeGrade)
    {
        GradeData gradeData = GradeDatabase.Instance.GetGrade(youtubeGrade);

        if (gradeData == null)
            return;

        int donation = Mathf.RoundToInt(
            foodCost * gradeData.DonationBonus);

        CurrencyManager.Instance.AddGold(donation);
    }
}