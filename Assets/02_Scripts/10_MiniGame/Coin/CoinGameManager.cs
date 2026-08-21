using UnityEngine;
using TMPro;
public class CoinGameManager : MonoBehaviour
{
    [Header("Game Setting")]
    [SerializeField] private float gameDuration = 15f;
    private float remainingTime;
    private int totalCoin;
    [Header("UI")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject totalPanel;
    [SerializeField] private TMP_Text totalPoint;

    private bool isMiniGamePlaying;
    public bool IsMiniGamePlaying => isMiniGamePlaying;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isMiniGamePlaying)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            EndGame();
        }
        UpdateTimeUI();
    }

    public void StartGame()
    {
        totalCoin = 0;
        remainingTime = gameDuration;

        isMiniGamePlaying = true;

        UpdateTimeUI();
    }
    private void UpdateTimeUI()
    {
        timeText.text = $"{Mathf.Max(remainingTime, 0f).ToString("F1")}";
    }

    public void AddCoin(int value)
    {
        if (!isMiniGamePlaying)
        {
            return;
        }

        totalCoin += value;

        Debug.Log($"ÄÚÀÎ È¹µæ: {value} / ÇöÀç Á¡¼ö: {totalCoin}");
    }

    private void EndGame()
    {
        isMiniGamePlaying = false;
        totalPoint.text = $"ÃÑ Á¡¼ö: {totalCoin}";
        totalPanel.SetActive(true);
    }
}