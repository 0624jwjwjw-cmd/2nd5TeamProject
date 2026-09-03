using System.Collections;
using UnityEngine;

public class MiniGameStarter : MonoBehaviour
{
    [Header("Game Panel")]
    [SerializeField] private GameObject gamePanel;

    [Header("Mini Game Panels")]
    [SerializeField] private GameObject[] miniGamePanels;

    [Header("Mini Game Manager")]
    [SerializeField] private MiniGameManager miniGameManager;

    [Header("Roulette")]
    [SerializeField] private GameObject roulettePanel;

    [Header("Panel Close")]
    [SerializeField] private float closeDelay = 1f;
    private int totalDonation;
    private int totalSubscribers;
    private GameObject currentMiniGamePanel;
    private Coroutine closeCoroutine;
    private MiniGameRoulette roulette;
    private void Awake()
    {
        roulette = roulettePanel.GetComponent<MiniGameRoulette>();
    }
    private void OnEnable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged += HandleMiniGameState;
        }
        if (roulette != null)
        {
            roulette.OnRouletteFinished += StartSelectedGame;
        }
    }

    private void OnDisable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged -= HandleMiniGameState;
        }
        if (roulette != null)
        {
            roulette.OnRouletteFinished -= StartSelectedGame;
        }
    }

    public void StartMiniGame(int totalDonation, int totalSubscribers)
    {
        this.totalDonation = totalDonation;
        this.totalSubscribers = totalSubscribers;
        // 이미 미니게임이 진행 중이면 실행하지 않음
        if (miniGameManager.IsMiniGamePlaying)
        {
            return;
        }
        if (Random.value > 0.5f)
        {
            return;
        }
        // 기존 패널 정리
        ResetPanels();



        // 3개 중 하나 랜덤 선택
        int randomIndex = Random.Range(0, miniGamePanels.Length);

        currentMiniGamePanel = miniGamePanels[randomIndex];

        // 선택된 게임 패널 활성화
        roulettePanel.SetActive(true);
        roulette.StartRoulette(randomIndex);
    }
    private void StartSelectedGame(int gameIndex)
    {
        roulettePanel.SetActive(false);

        gamePanel.SetActive(true);
        currentMiniGamePanel = miniGamePanels[gameIndex];
        currentMiniGamePanel.SetActive(true);

        miniGameManager.StartGame(totalDonation, totalSubscribers, gameIndex);
    }
    private void HandleMiniGameState(bool isPlaying)
    {
        // 게임 시작 상태는 여기서 할 일 없음
        if (isPlaying)
        {
            return;
        }

        // 게임 종료
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        closeCoroutine = StartCoroutine(ClosePanelAfterDelay());
    }

    private IEnumerator ClosePanelAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        ResetPanels();

        closeCoroutine = null;
    }

    private void ResetPanels()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        if (roulettePanel != null)
        {
            roulettePanel.SetActive(false);
        }

        foreach (GameObject panel in miniGamePanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        currentMiniGamePanel = null;
    }
    public void StopMiniGame()
    {
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        if (miniGameManager != null)
        {
            miniGameManager.StopGame();
        }

        ResetPanels();
    }
}