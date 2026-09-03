using System;
using TMPro;
using UnityEngine;
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
    private float warningTimer;
    private bool isMiniGamePlaying;
    public bool IsMiniGamePlaying => isMiniGamePlaying;
    public event Action<bool> OnMiniGamePlayingChanged;
    private int totalDonation=0;
    private int totalSubscribers=0;
    private int gameType;
    private void SetMiniGamePlaying(bool value)
    {
        if (isMiniGamePlaying == value)
        {
            return;
        }

        isMiniGamePlaying = value;

        OnMiniGamePlayingChanged?.Invoke(isMiniGamePlaying);
    }

    private void Update()
    {
        if (!isMiniGamePlaying)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 5f)
        {
            warningTimer -= Time.deltaTime;

            if (warningTimer <= 0f)
            {
                warningTimer = 1f;

                SoundManager.Instance.PlaySFX(SFXType.MTimer);
            }
        }
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            EndGame();
        }
        UpdateTimeUI();
    }

    public void StartGame(int gameType)
    {

        this.gameType = gameType;
        totalCoin = 0;
        warningTimer = 0f;
        remainingTime = gameDuration;
        totalPanel.SetActive(false);
        scoreText.text = "점수 : 0점";
        SetMiniGamePlaying(true);
        UpdateTimeUI();
    }
    public void GetReward(int totalDonation, int totalSubscribers)
    {
        this.totalDonation += totalDonation;
        this.totalSubscribers += totalSubscribers;
    }
    private void UpdateTimeUI()
    {
        timeText.text = $"{Mathf.Max(remainingTime, 0f).ToString("F1")}";
    }
    public void PointReward()
    {
        int moneyReward = 0;
        int subscribersReward = 0;
        float bonusRate = totalCoin * 0.01f;
        switch (gameType)
        {
            case 0://코인
                moneyReward = Mathf.RoundToInt(totalDonation * bonusRate);
                totalPoint.text = $"총 점수: {totalCoin}점 \n추가 후원금 : {moneyReward}원";
                break;
            case 1://맥주
                subscribersReward = Mathf.RoundToInt(totalSubscribers * bonusRate);
                totalPoint.text = $"총 점수: {totalCoin}점 \n추가 구독자 : {subscribersReward}명";
                break;
            case 2://먹방
                moneyReward = Mathf.RoundToInt(totalDonation * bonusRate);
                subscribersReward = Mathf.RoundToInt(totalSubscribers * bonusRate);
                totalPoint.text = $"총 점수: {totalCoin}점 \n추가 후원금 : {moneyReward}원\n추가 구독자 : {subscribersReward}명";
                break;
        }
        if (moneyReward > 0)
        {
            CurrencyManager.Instance.AddGold(moneyReward);
        }
        if (subscribersReward > 0)
        {
            CurrencyManager.Instance.AddSubscriber(subscribersReward);
        }
    }
    public void AddCoin(int value)
    {
        if (!isMiniGamePlaying)
        {
            return;
        }
        totalCoin += value;
        if (totalCoin <= 0)
        {
            totalCoin = 0;
        }
        scoreText.text = $"점수 : {totalCoin}점";
    }

    private void EndGame()
    {
        SoundManager.Instance.PlaySFX(SFXType.Win);
        PointReward();
        totalPanel.SetActive(true);
        totalDonation = 0;
        totalSubscribers = 0;
        SetMiniGamePlaying(false);
    }
    public void StopGame()
    {
        if (!isMiniGamePlaying)
        {
            return;
        }
        totalDonation = 0;
        totalSubscribers = 0;
        totalPanel.SetActive(false);
        SetMiniGamePlaying(false);
    }
}