using UnityEngine;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    [SerializeField] private Image rankImage;
    [SerializeField] private Sprite[] rankSprites;

    private SubscriberRank rankManager;


    private void Start()
    {
        rankManager = FindFirstObjectByType<SubscriberRank>();

        rankManager.OnRankChanged += ChangeRankImage;
        ChangeRankImage(rankManager.CurrentRank);
    }


    private void OnDestroy()
    {
        if (rankManager != null)
        {
            rankManager.OnRankChanged -= ChangeRankImage;
        }
    }


    private void ChangeRankImage(int rank)
    {
        rankImage.sprite = rankSprites[rank - 1];
    }
}