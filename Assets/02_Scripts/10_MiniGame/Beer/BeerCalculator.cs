using System.Collections;
using TMPro;
using UnityEngine;

public class BeerCalculator : MonoBehaviour
{
    [Header("Beer")]
    [SerializeField] private BeerController beerController;

    [Header("Target")]
    [SerializeField] private BeerTargetArrow targetArrow;

    [Header("Manager")]
    [SerializeField] private MiniGameManager gameManager;

    [Header("Result UI")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject tooMuchImage;

    [Header("Score")]
    [SerializeField] private int perfectScore = 5;
    [SerializeField] private int goodScore = 3;
    [SerializeField] private int outScore = -3;

    [Header("Result Setting")]
    [SerializeField] private float resultDisplayTime = 1f;
    [SerializeField] private float tooMuchDisplayTime = 0.5f;

    private Coroutine resultCoroutine;
    private Coroutine tooMuchCoroutine;

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

    private void HandleGameState(bool isPlaying)
    {
        if (isPlaying)
        {
            ResetUI();
        }
        else
        {
            ResetUI();
        }
    }

    public void CalculateScore()
    {
        // 게임이 끝난 상태에서는 계산하지 않음
        if (!gameManager.IsMiniGamePlaying)
        {
            return;
        }

        float beerAmount = beerController.GetBeerAmount();
        float targetAmount = targetArrow.GetTargetPercent();

        float ratio = beerAmount / targetAmount;

        int score;

        if (beerAmount >= 0.95f)
        {
            score = outScore;
            SoundManager.Instance.PlaySFX(SFXType.MBad);
            resultText.text = "TooMuch!";
            resultText.color = Color.red;

            if (tooMuchCoroutine != null)
            {
                StopCoroutine(tooMuchCoroutine);
            }

            tooMuchCoroutine = StartCoroutine(ShowTooMuch());
        }
        else if (ratio >= 0.9f && ratio <= 1.1f)
        {
            score = perfectScore;
            SoundManager.Instance.PlaySFX(SFXType.MGood);
            resultText.text = "PERFECT!";
            resultText.color = Color.green;
        }
        else if (ratio >= 0.75f && ratio <= 1.25f)
        {
            score = goodScore;
            SoundManager.Instance.PlaySFX(SFXType.MGood);
            resultText.text = "GOOD";
            resultText.color = Color.yellow;
        }
        else
        {
            score = outScore;
            SoundManager.Instance.PlaySFX(SFXType.MBad);
            resultText.text = "OUT...";
            resultText.color = Color.red;
        }

        gameManager.AddCoin(score);

        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
        }

        resultCoroutine = StartCoroutine(ShowResult());
    }

    private IEnumerator ShowResult()
    {
        resultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(resultDisplayTime);

        resultText.gameObject.SetActive(false);

        resultCoroutine = null;
    }

    private IEnumerator ShowTooMuch()
    {
        tooMuchImage.SetActive(true);

        yield return new WaitForSeconds(tooMuchDisplayTime);

        tooMuchImage.SetActive(false);

        tooMuchCoroutine = null;
    }

    private void ResetUI()
    {
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
            resultCoroutine = null;
        }

        if (tooMuchCoroutine != null)
        {
            StopCoroutine(tooMuchCoroutine);
            tooMuchCoroutine = null;
        }

        resultText.gameObject.SetActive(false);
        tooMuchImage.SetActive(false);
    }
}