using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    [Header("Coin Setting")]
    [SerializeField] private float minFallSpeed = 500f;
    [SerializeField] private float maxFallSpeed = 1000f;
    [SerializeField] private float floorY = -1000f;

    [Header("Coin Type")]
    [SerializeField] private Image coinImage;
    [SerializeField] private Sprite goodSprite;
    [SerializeField] private Sprite badSprite;
    [SerializeField] private float goodChance = 0.8f;

    private RectTransform rectTransform;
    private CoinSpawner spawner;
    private float fallSpeed;

    private bool isGoodCoin;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(CoinSpawner spawner)
    {
        this.spawner = spawner;
        SetRandomFallSpeed();
        SetRandomCoinType();
    }
    public void SetRandomFallSpeed()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
    }
    private void SetRandomCoinType()
    {
        isGoodCoin = Random.value < goodChance;

        if (isGoodCoin)
        {
            coinImage.sprite = goodSprite;
        }
        else
        {
            coinImage.sprite = badSprite;
        }
    }
    private void Update()
    {
        rectTransform.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        if (spawner == null)
        {
            return;
        }

        if (rectTransform.anchoredPosition.y <= floorY)
        {
            spawner.ReleaseCoin(this);
        }
    }
    public void Release()
    {
        if (spawner == null)
        {
            return;
        }

        spawner.ReleaseCoin(this);
    }
    public int GetCoinValue()
    {
        return isGoodCoin ? 1 : -1;
    }
}