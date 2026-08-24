using UnityEngine;
using TMPro;
public class MiniGameManager : MonoBehaviour
{
    [Header("Game Setting")]
    [SerializeField] private float gameDuration = 15f;
    private float remainingTime;
    private int totalCoin;
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [Header("PanelUI")]
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
        scoreText.text = $"점수 : {totalCoin}점";
    }

    private void EndGame()
    {
        isMiniGamePlaying = false;
        totalPoint.text = $"총 점수: {totalCoin}";
        totalPanel.SetActive(true);
    }
}