using System.Collections;
using TMPro;
using UnityEngine;

public class CoinText : MonoBehaviour
{
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float moveUpDistance = 50f;

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

        while (elapsedTime < displayTime)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / displayTime;

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}