using TMPro;
using UnityEngine;

public class GameDateUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text countText;

    private void Start()
    {
        GameDateManager.Instance.OnDateChanged += RefreshUI;
    }
    private void OnDestroy()
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

        dayText.text = $"{currentDay}ÀÏÂ÷";
        countText.text = $"{currentCount} / {maxCount}";
    }
}