using UnityEngine;

public class SinkZone : MonoBehaviour
{
    [Header("Sink Type")]
    [SerializeField] private bool isSinkA;

    [Header("Score")]
    [SerializeField] private int correctScore = 3;
    [SerializeField] private int wrongScore = -3;

    [Header("Game Manager")]
    [SerializeField] private FoodGameManager foodGameManager;
    [Header("Mini Game Manager")]
    [SerializeField] private MiniGameManager miniGameManager;

    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public bool IsInside(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform,screenPosition,null);
    }
    public void CheckBowl(FoodBowl bowl)
    {
        if (bowl == null || !bowl.IsEmpty)
        {
            return;
        }
        SoundManager.Instance.PlaySFX(SFXType.MSink);
        bool isCorrect = bowl.IsBowlA == isSinkA;

        if (isCorrect)
        {
            miniGameManager.AddCoin(correctScore);
        }
        else
        {
            miniGameManager.AddCoin(wrongScore);
        }

        Destroy(bowl.gameObject);
        foodGameManager.OnBowlWashed(bowl);
    }
}
