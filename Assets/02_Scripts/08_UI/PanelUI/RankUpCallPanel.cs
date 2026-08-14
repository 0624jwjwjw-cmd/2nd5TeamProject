using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpCallPanel : MonoBehaviour
{
    [SerializeField] private GameObject rankPanel;
    [SerializeField] private GameObject darkPanel;

    [SerializeField] private Image rankImage;
    [SerializeField] private Sprite[] rankSprites;
    [SerializeField] private TMP_Text rankText;

    private SubscriberRank rankManager;

    public void OpenPanel()
    {
        rankPanel.SetActive(true);

        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        rankPanel.SetActive(false);

        if (darkPanel != null)
        {
            darkPanel.SetActive(false);
        }
    }

    private void Start()
    {
        rankManager = FindFirstObjectByType<SubscriberRank>();

        if (rankManager != null)
        {
            rankManager.OnRankChanged += ShowRankUpPanel;
        }
    }

    private void OnDestroy()
    {
        if (rankManager != null)
        {
            rankManager.OnRankChanged -= ShowRankUpPanel;
        }
    }

    // 승급 이벤트를 받으면 실행
    private void ShowRankUpPanel(int newRank)
    {
        ChangeRankImage(newRank);

        rankText.text = $"축하합니다!\r\n{RankName(newRank)}로 승급하셨습니다!";

        OpenPanel();
    }
    
    // 현재 랭크에 맞는 이미지로 변경
    private void ChangeRankImage(int rank)
    {
        if (rank < 1 || rank > rankSprites.Length)
            return;

        rankImage.sprite = rankSprites[rank - 1];
    }
    private string RankName(int newRank)
    {
        return newRank switch
        {
            1 => "브론즈",
            2 => "실버",
            3 => "골드",
            4 => "다이아몬드",
            5 => "레드",
            _ => "알 수 없음"
        };
    }
}