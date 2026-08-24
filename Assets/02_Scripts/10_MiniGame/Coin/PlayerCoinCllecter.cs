using UnityEngine;

public class PlayerCoinCollector : MonoBehaviour
{
    [SerializeField] private MiniGameManager gameManager;
    [SerializeField] private CoinText resultTextPrefab;
    [SerializeField] private Transform resultTextRoot;
    [SerializeField] private Vector2 resultTextOffset = new Vector2(0f, 100f);
    private RectTransform playerRect;

    private void Awake()
    {
        playerRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Coin[] coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);

        foreach (Coin coin in coins)
        {
            if (!coin.gameObject.activeSelf)
                continue;

            RectTransform coinRect = coin.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(playerRect,coinRect.position))
            {
                CollectCoin(coin);
            }
        }
    }
    private void ShowResultText(int value)
    {
        CoinText resultText =
         Instantiate(resultTextPrefab, resultTextRoot);

        RectTransform resultRect =
            resultText.GetComponent<RectTransform>();

        RectTransform playerRect =
            GetComponent<RectTransform>();

        resultRect.anchoredPosition =
            playerRect.anchoredPosition + resultTextOffset;

        resultText.Show(value);
    }
    private void CollectCoin(Coin coin)
    {
        int value = coin.GetCoinValue();
        ShowResultText(value);
        coin.Release();
        gameManager.AddCoin(value);
    }
}