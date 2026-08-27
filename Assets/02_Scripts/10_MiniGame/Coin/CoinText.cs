using System.Collections;
using TMPro;
using UnityEngine;

public class CoinText : MonoBehaviour
{
    [SerializeField] private float moveUpDistance = 50f;
    [SerializeField] private float moveDuration = 0.5f;

    private TMP_Text resultText;
    private RectTransform rectTransform;

    private void Awake()
    {
        resultText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(int value)
    {
        resultText.text = value > 0 ? "+1" : "-1";

        StartCoroutine(ResultRoutine());
    }

    private IEnumerator ResultRoutine()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition =
            startPosition + Vector2.up * moveUpDistance;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition, endPosition, t);

            yield return null;
        }
    }
}