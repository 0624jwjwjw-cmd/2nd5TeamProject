using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private Coin coinPrefab;
    [Header("Spawn")]
    [SerializeField] private Transform spawnRoot;
    [Header("Pool")]
    [SerializeField] private Transform poolRoot;
    private ComponentPool<Coin> coinPool;

    [Header("Spawn Setting")]
    [SerializeField] private float spawnY = 500f;
    [SerializeField] private float spawnInterval = 0.3f;
    private RectTransform spawnArea;

    [SerializeField] private CoinGameManager gameManager;
    private List<Coin> activeCoins = new List<Coin>();

    private void Awake()
    {
        coinPool = new ComponentPool<Coin>(coinPrefab,poolRoot);
        spawnArea = spawnRoot.GetComponent<RectTransform>();
    }
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnCoin();

            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void SpawnCoin()
    {
        if (!gameManager.IsMiniGamePlaying)
        {
            ReleaseAllCoins();
            return;
        }

        Coin coin = coinPool.Get(spawnRoot);
        RectTransform coinRect = coin.GetComponent<RectTransform>();

        // 패널의 절반 너비
        float areaHalfWidth = spawnArea.rect.width * 0.5f;
        // 코인의 절반 너비
        float coinHalfWidth = coinRect.rect.width * 0.5f;
        // 코인이 패널 밖으로 나가지 않도록 X 범위 계산
        float minX = -areaHalfWidth + coinHalfWidth;
        float maxX = areaHalfWidth - coinHalfWidth;
        // 계산된 범위 안에서 랜덤 X
        float randomX = Random.Range(minX, maxX);
        // 코인 위치 설정
        coinRect.anchoredPosition = new Vector2(randomX,spawnY);
        coin.Initialize(this);
        coin.SetRandomFallSpeed();
        activeCoins.Add(coin);
    }
    public void ReleaseAllCoins()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = activeCoins[i];

            if (coin != null)
            {
                coinPool.Release(coin);
            }
        }

        activeCoins.Clear();
    }
    public void ReleaseCoin(Coin coin)
    {
        if (coin == null)
        {
            return;
        }
        activeCoins.Remove(coin);
        coinPool.Release(coin);
    }
}