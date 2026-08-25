using UnityEngine;

public class SinkZone : MonoBehaviour
{
    [Header("Sink Type")]
    [SerializeField] private bool isSinkA;

    [Header("Score")]
    [SerializeField] private int correctScore = 1;
    [SerializeField] private int wrongScore = -1;

    [Header("Game Manager")]
    [SerializeField] private FoodGameManager foodGameManager;

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

        bool isCorrect = bowl.IsBowlA == isSinkA;

        if (isCorrect)
        {
            Debug.Log($"정답! +{correctScore}");
        }
        else
        {
            Debug.Log($"오답! {wrongScore}");
        }

        Destroy(bowl.gameObject);
        foodGameManager.OnBowlWashed(bowl);
    }
}
