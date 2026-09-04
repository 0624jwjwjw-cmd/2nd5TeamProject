using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiveResultUI : MonoBehaviour
{
    [SerializeField] private LiveManager liveManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text donationText;
    [SerializeField] private TMP_Text subscriberText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (liveManager != null)
            liveManager.OnLiveEnded += ShowResult;
    }

    private void OnDestroy()
    {
        if (liveManager != null)
            liveManager.OnLiveEnded -= ShowResult;

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    private void ShowResult()
    {
        if (panel != null)
            panel.SetActive(true);

        if (donationText != null)
            donationText.text =
                $"후원금 +{liveManager.TotalDonation}";

        if (subscriberText != null)
            subscriberText.text =
                $"구독자 +{liveManager.TotalSubscribers}";
    }

    private void Close()
    {
        SoundManager.Instance?.PlaySFX(SFXType.ButtonClick);

        if (panel != null)
            panel.SetActive(false);
    }
}