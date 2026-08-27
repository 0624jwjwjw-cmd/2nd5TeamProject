using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameRoulette : MonoBehaviour
{
    [Header("Game Image")]
    [SerializeField] private Image gameImage;

    [SerializeField] private Sprite coinGameSprite;
    [SerializeField] private Sprite beerGameSprite;
    [SerializeField] private Sprite foodGameSprite;

    [Header("Setting")]
    [SerializeField] private float rouletteTime = 2f;
    [SerializeField] private float changeInterval = 0.1f;

    public event Action<int> OnRouletteFinished;

    private int selectedGame;

    public void StartRoulette(int gameIndex)
    {
        selectedGame = gameIndex;

        StartCoroutine(RouletteRoutine());
    }

    private IEnumerator RouletteRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < rouletteTime)
        {
            ShowRandomImage();

            yield return new WaitForSeconds(changeInterval);

            elapsedTime += changeInterval;
        }

        // 최종 선택된 게임 표시
        ShowGameImage(selectedGame);

        // 최종 이미지가 1초 동안 보이도록 대기
        yield return new WaitForSeconds(1f);

        // Starter에게 완료 알림
        OnRouletteFinished?.Invoke(selectedGame);
    }

    private void ShowRandomImage()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3);

        ShowGameImage(randomIndex);
    }

    private void ShowGameImage(int index)
    {
        switch (index)
        {
            case 0:
                gameImage.sprite = coinGameSprite;
                break;

            case 1:
                gameImage.sprite = beerGameSprite;
                break;

            case 2:
                gameImage.sprite = foodGameSprite;
                break;
        }
    }
}