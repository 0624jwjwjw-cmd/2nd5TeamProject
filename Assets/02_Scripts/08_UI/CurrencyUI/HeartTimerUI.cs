using TMPro;
using UnityEngine;

public class HeartTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        if (HeartManager.Instance == null)
            return;
        
        RefreshTimer();
    }
    private void RefreshTimer()
    {
        int remaining = HeartManager.Instance.GetRemainingRecoverTime();

        if (CurrencyManager.Instance.Heart >= 10)
        {
            timerText.text = "MAX";
            return;
        }

        int minute = remaining / 60;
        int second = remaining % 60;

        timerText.text =$"{minute:00}:{second:00}";
    }
}
