using UnityEngine;
using UnityEngine.UI;
public class BeerFoamController : MonoBehaviour
{
    [Header("Beer")]
    [SerializeField] private BeerController beerController;

    [Header("Foam")]
    [SerializeField] private RectTransform foam;
    [SerializeField] private Image beerImage;

    private float originalFoamScaleY;
    private RectTransform beerRect;

    private void Start()
    {
        originalFoamScaleY = foam.localScale.y;
        beerRect = beerImage.GetComponent<RectTransform>();
    }

    private void Update()
    {
        float beerAmount = beerController.GetBeerAmount();

        UpdateFoam(beerAmount);
    }

    private void UpdateFoam(float beerAmount)
    {
        float foamRatio;

        // 0% ~ 20%
        if (beerAmount < 0.2f)
        {
            foamRatio = beerAmount / 0.2f;
        }
        // 20% ~ 80%
        else if (beerAmount < 0.8f)
        {
            foamRatio = 1f;
        }
        // 80% ~ 100%
        else
        {
            foamRatio = 1f - ((beerAmount - 0.8f) / 0.2f);
        }

        Vector3 scale = foam.localScale;
        scale.y = originalFoamScaleY * foamRatio;
        foam.localScale = scale;

        float beerHeight = beerRect.rect.height;

        float beerY = beerRect.rect.yMin + beerHeight * beerAmount;

        Vector2 position = foam.anchoredPosition;
        position.y = beerY;
        foam.anchoredPosition = position;
    }
}