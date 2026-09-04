using UnityEngine;

public class BeerTargetArrow : MonoBehaviour
{
    [Header("Gauge")]
    [SerializeField] private RectTransform gauge;

    [Header("Arrow")]
    [SerializeField] private RectTransform arrow;
    private float targetPercent;
    public void SetRandomPosition()
    {
        targetPercent = Random.Range(0.6f, 0.9f);
        float height = gauge.rect.height;

        float y = (targetPercent - 0.5f) * height;

        Vector2 position = arrow.anchoredPosition;
        position.y = y;

        arrow.anchoredPosition = position;
    }
    public float GetTargetPercent()
    {
        return targetPercent;
    }
}
