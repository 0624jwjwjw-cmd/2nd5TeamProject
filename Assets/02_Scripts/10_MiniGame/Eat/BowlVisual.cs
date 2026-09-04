using UnityEngine;
using UnityEngine.UI;

public class BowlVisual : MonoBehaviour
{
    [Header("Food Image")]
    [SerializeField] private Image foodImage;
    [SerializeField] private Sprite bowlAFoodSprite;
    [SerializeField] private Sprite bowlBFoodSprite;
    [Header("Empty Bowl")]
    [SerializeField] private Sprite bowlImage;
    [Header("Empty Bowl Color")]
    [SerializeField] private Color bowlAColor = Color.blue;
    [SerializeField] private Color bowlBColor = Color.red;
    public void SetFoodVisual(bool isBowlA)
    {
        foodImage.sprite = isBowlA? bowlAFoodSprite: bowlBFoodSprite;

        // 음식일 때는 원래 색상 유지
        foodImage.color = Color.white;
    }
    public void SetFoodFill(float amount)
    {
        foodImage.fillAmount = amount;
    }

    public void SetEmptyVisual(bool isBowlA)
    {
        foodImage.sprite = bowlImage;
        foodImage.fillAmount = 1f;

        foodImage.color = isBowlA ? bowlAColor : bowlBColor;
    }
}