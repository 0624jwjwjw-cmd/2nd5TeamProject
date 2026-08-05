using TMPro;
using UnityEngine;

public class RevenueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text subscriberText;

    //이벤트 보고용
    private void OnEnable()
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager가 아직 생성되지 않음");
            return;
        }
        CurrencyManager.Instance.OnRevenueChanged += RefreshUI;
    }
    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged -= RefreshUI;
        }
    }

    private void Start()
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        goldText.text = $"Gold : {CurrencyManager.Instance.Gold:N0}Won";

        subscriberText.text = $"Subscriber : {FormatSubscriber(CurrencyManager.Instance.Subscriber)} people";
    }

    private string FormatSubscriber(int value)
    {
        if (value >= 1000)
        {
            float man = value / 1000f;

            return $"{man:0.0}K";
        }

        return value.ToString();
    }
}
