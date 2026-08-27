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

    [Header("Panel Close")]
    [SerializeField] private float closeDelay = 1f;

    private GameObject currentMiniGamePanel;
    private Coroutine closeCoroutine;

    private void OnEnable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged += HandleMiniGameState;
        }
    }

    private void OnDisable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged -= HandleMiniGameState;
        }
    }

    public void StartMiniGame()
    {
        // 이미 미니게임이 진행 중이면 실행하지 않음
        if (miniGameManager.IsMiniGamePlaying)
        {
            return;
        }

        // 기존 패널 정리
        ResetPanels();

        // 전체 게임 패널 활성화
        gamePanel.SetActive(true);

        // 3개 중 하나 랜덤 선택
        int randomIndex = Random.Range(0, miniGamePanels.Length);

        currentMiniGamePanel = miniGamePanels[randomIndex];

        // 선택된 게임 패널 활성화
        currentMiniGamePanel.SetActive(true);

        // 미니게임 시작
        miniGameManager.StartGame();
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

        foreach (GameObject panel in miniGamePanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        currentMiniGamePanel = null;
    }
}