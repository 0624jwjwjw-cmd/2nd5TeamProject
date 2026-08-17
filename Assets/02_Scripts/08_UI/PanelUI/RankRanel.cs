using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankRanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject rankPanel;
    [SerializeField] private GameObject darkPanel;

    [Header("Subscriber")]
    [SerializeField] private TMP_Text currentSubscriberText;
    [SerializeField] private TMP_Text nextSubscriberText;
    [SerializeField] private Slider progressSlider;

    [Header("LockImage")]
    [SerializeField] private Image[] lockImages;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite unlockSprite;

    private SubscriberRank rankManager;

    public void OpenPanel()
    {
        rankPanel.SetActive(true);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }
    }
    public void ClosePanel()
    {
        rankPanel.SetActive(false);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (darkPanel != null)
        {
            darkPanel.SetActive(false);
        }
    }

    private void Start()
    {
        rankManager = FindFirstObjectByType<SubscriberRank>();
        CurrencyManager.Instance.OnRevenueChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged -= RefreshUI;
        }
    }

    private void RefreshUI()
    {
        if (CurrencyManager.Instance == null ||
            rankManager == null)
            return;

        int subscriber = CurrencyManager.Instance.Subscriber;
        int currentRank = rankManager.CurrentRank;

        // 현재 구독자
        currentSubscriberText.text = $"현재 구독자{subscriber:N0}명";

        // 잠금 이미지
        UpdateLockImages(currentRank);

        // 최고 티어
        if (currentRank == 5)
        {
            nextSubscriberText.text = "MAX";
            progressSlider.value = 1f;
            return;
        }

        int currentRequirement = GetNextRankRequirement(currentRank - 1);
        int nextRequirement = GetNextRankRequirement(currentRank);

        int remainingSubscriber = Mathf.Max(0, nextRequirement - subscriber);

        nextSubscriberText.text = $"다음 티어까지 {remainingSubscriber:N0}명";

        float currentSubscriber = subscriber - currentRequirement;
        float nextSubscriber = nextRequirement - currentRequirement;

        progressSlider.value = Mathf.Clamp01(currentSubscriber / nextSubscriber);
    }

    private void UpdateLockImages(int currentRank)
    {
        for (int i = 0; i < lockImages.Length; i++)
        {
            int rank = i + 1;

            if (rank <= currentRank)
            {
                lockImages[i].sprite = unlockSprite;
            }
            else
            {
                lockImages[i].sprite = lockSprite;
            }
        }
    }

    private int GetNextRankRequirement(int currentRank)
    {
        switch (currentRank)
        {
            case 0: return 0;
            case 1: return 5000;
            case 2: return 30000;
            case 3: return 150000;
            case 4: return 500000;
            default: return -1;
        }
    }
}
