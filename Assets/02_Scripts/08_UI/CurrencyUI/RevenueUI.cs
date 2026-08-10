using TMPro;
using UnityEngine;

public class RevenueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text subscriberText;

    //이벤트 보고용
    private void Start()
    {
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager없엉");
            return;
        }
        CurrencyManager.Instance.OnRevenueChanged += RefreshUI;
        RefreshUI();
    }
    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged -= RefreshUI;
        }
    }
    private void RefreshUI()
    {
        goldText.text = $"후원금 : {CurrencyManager.Instance.Gold:N0}원";

        subscriberText.text = $"구독자 : {FormatSubscriber(CurrencyManager.Instance.Subscriber)} 명";
    }

    private string FormatSubscriber(int value)
    {
        if (value >= 10000)
        {
            float man = value / 10000f;

            return $"{man:0.0}만";
        }

        return value.ToString();
    }
}
