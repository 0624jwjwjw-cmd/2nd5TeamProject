using UnityEngine;
using UnityEngine.UI;
public class BeerController : MonoBehaviour
{
    [Header("Beer")]
    [SerializeField] private Image beerImage;

    [Header("Pour Setting")]
    [SerializeField] private float minPourSpeed = 0.6f;
    [SerializeField] private float maxPourSpeed = 2.0f;

    [Header("Manager")]
    [SerializeField] private MiniGameManager gameManager;
    [SerializeField] private BeerTargetArrow targetArrow;
    [SerializeField] private BeerCalculator scoreCalculator;

    private float pourSpeed;
    private bool wasPressed;

    private void Start()
    {
        StartNewRound();
    }
    private void Update()
    {
        if (!gameManager.IsMiniGamePlaying)
        {
            wasPressed = false;
            return;
        }
        // InputManager가 없으면 정지
        if (InputManager.Instance == null)
            return;

        bool isPressed = InputManager.Instance.IsPressed;

        if (isPressed)
        {
            PourBeer();
        }

        // 누르고 있다가 이번 프레임에 뗀 순간
        if (wasPressed && !isPressed)
        {
            scoreCalculator.CalculateScore();
            StartNewRound();
        }
        wasPressed = isPressed;
    }
    private void PourBeer()
    {
        beerImage.fillAmount += pourSpeed * Time.deltaTime;
        
        // 100% 이상 올라가지 않도록 제한
        if (beerImage.fillAmount >= 1f)
        {
            beerImage.fillAmount = 1f;
        }
    }
    private void StartNewRound()
    {
        ResetBeer();
        pourSpeed = Random.Range(minPourSpeed, maxPourSpeed);
        targetArrow.SetRandomPosition();
    }
    public float GetBeerAmount()
    {
        return beerImage.fillAmount;
    }

    public void ResetBeer()
    {
        beerImage.fillAmount = 0f;
    }
}
