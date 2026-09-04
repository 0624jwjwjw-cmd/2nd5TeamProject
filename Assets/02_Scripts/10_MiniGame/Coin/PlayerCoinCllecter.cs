using UnityEngine;

public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private MiniGameManager gameManager;
    [SerializeField] private CoinSpawner coinSpawner;
    [Header("Result Text")]
    [SerializeField] private CoinText resultTextPrefab;
    [SerializeField] private Transform resultTextRoot;
    [SerializeField] private Vector2 resultTextOffset = new Vector2(0f, 100f);

    private RectTransform playerRect;
    private bool isPlaying;

    private void Awake()
    {
        playerRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.OnMiniGamePlayingChanged += HandleGameState;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnMiniGamePlayingChanged -= HandleGameState;
        }
    }

    private void HandleGameState(bool playing)
    {
        isPlaying = playing;
    }

    private void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        for (int i = coinSpawner.ActiveCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = coinSpawner.ActiveCoins[i];

            if (!coin.gameObject.activeSelf)
            {
                continue;
            }

            RectTransform coinRect = coin.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(
                playerRect,
                coinRect.position))
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

        resultRect.anchoredPosition =
            playerRect.anchoredPosition + resultTextOffset;

        resultText.Show(value);
    }

    private void CollectCoin(Coin coin)
    {
        int value = coin.GetCoinValue();

        ShowResultText(value);
        if (value > 0)
        {
            SoundManager.Instance.PlaySFX(SFXType.MCoin);
        }
        else
        {
            SoundManager.Instance.PlaySFX(SFXType.MBad);
        }
        coin.Release();

        gameManager.AddCoin(value);
    }
}