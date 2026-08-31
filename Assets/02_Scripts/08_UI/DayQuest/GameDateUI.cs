using TMPro;
using UnityEngine;

public class GameDateUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text countText;

    private void OnEnable()
    {
        if (GameDateManager.Instance != null)
        {
            GameDateManager.Instance.OnDateChanged += RefreshUI;

            // 현재 날짜를 바로 갱신
            RefreshUI(GameDateManager.Instance.DateCount);
        }
    }

    private void OnDisable()
    {
        if (GameDateManager.Instance != null)
        {
            GameDateManager.Instance.OnDateChanged -= RefreshUI;
        }
    }

    private void RefreshUI(int dayCount)
    {
        int maxCount = GameDateManager.Instance.MaxDateCount;

        int currentDay = dayCount / maxCount + 1;
        int currentCount = dayCount % maxCount;

        dayText.text = $"{currentDay}일차";
        countText.text = $"{currentCount} / {maxCount}";
    }
}