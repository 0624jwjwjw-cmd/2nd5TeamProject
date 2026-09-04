using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeerPourVisual : MonoBehaviour
{
    [Header("Beer")]
    [SerializeField] private Image beerImage;

    [Header("Setting")]
    [SerializeField] private float pourSpeed = 1f;
    [SerializeField] private float drainSpeed = 4f;

    private Coroutine drainCoroutine;

    private void Update()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        if (InputManager.Instance.IsPressed)
        {
            Pour();
        }
        else
        {
            Drain();
        }
    }

    private void Pour()
    {
        // 혹시 빠지는 중이었다면 중단
        if (drainCoroutine != null)
        {
            StopCoroutine(drainCoroutine);
            drainCoroutine = null;
        }

        // 위에서 아래로 떨어짐
        beerImage.fillOrigin = (int)Image.OriginVertical.Top;

        beerImage.fillAmount += pourSpeed * Time.deltaTime;
        beerImage.fillAmount = Mathf.Clamp01(beerImage.fillAmount);
    }

    private void Drain()
    {
        if (beerImage.fillAmount <= 0f)
        {
            return;
        }

        if (drainCoroutine == null)
        {
            drainCoroutine = StartCoroutine(DrainRoutine());
        }
    }

    private IEnumerator DrainRoutine()
    {
        // 현재 떨어지고 있던 양을 기억
        float currentFill = beerImage.fillAmount;

        // 아래쪽 기준으로 변경
        beerImage.fillOrigin = (int)Image.OriginVertical.Bottom;

        // 같은 양에서 시작
        beerImage.fillAmount = currentFill;

        while (beerImage.fillAmount > 0f)
        {
            beerImage.fillAmount -= drainSpeed * Time.deltaTime;

            yield return null;
        }

        beerImage.fillAmount = 0f;
        drainCoroutine = null;
    }
}