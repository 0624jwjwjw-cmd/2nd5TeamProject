using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text targetGoldText;
    [SerializeField] private TMP_Text earnedGoldText;

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged += RefreshUI;
        }

        if (GameDateManager.Instance != null)
        {
            GameDateManager.Instance.OnDateChanged += RefreshQuestUI;
        }

        // 현재 상태를 바로 갱신
        RefreshUI();
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged -= RefreshUI;
        }

        if (GameDateManager.Instance != null)
        {
            GameDateManager.Instance.OnDateChanged -= RefreshQuestUI;
        }
    }

    private void RefreshQuestUI(int dayCount)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (DayQuest.Instance == null)
            return;

        targetGoldText.text =
            $"다음날까지 금액 : {DayQuest.Instance.CurrentTargetGold:N0}원";

        earnedGoldText.text =
            $"오늘 번 금액 : {DayQuest.Instance.TodayEarnedGold:N0}원";
    }
}