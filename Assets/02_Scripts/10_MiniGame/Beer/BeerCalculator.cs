using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
    [SerializeField] private int tooMuchScore = -8;

    [SerializeField] private float resultDisplayTime = 1f;
    [SerializeField] private float tooMuchDisplayTime = 0.5f;
    private Coroutine resultCoroutine;
    private Coroutine tooMuchCoroutine;
    public void CalculateScore()
    {
        float beerAmount = beerController.GetBeerAmount();
        float targetAmount = targetArrow.GetTargetPercent();

        // 맥주 높이 / 목표 높이
        float ratio = beerAmount / targetAmount;
        int score;
        if (beerAmount >= 0.95)
        {
            score = outScore;
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
            resultText.text = "PERFECT!";
            resultText.color = Color.green;
        }
        else if (ratio >= 0.75f && ratio <= 1.25f)
        {
            score = goodScore;
            resultText.text = "GOOD";
            resultText.color = Color.yellow;
        }
        else
        {
            score = outScore;
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
}
