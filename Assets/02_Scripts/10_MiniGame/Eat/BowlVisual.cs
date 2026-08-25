using System;
using UnityEngine;
using UnityEngine.UI;

public class BowlVisual : MonoBehaviour
{
    [Header("Bowl Color")]
    [SerializeField] private Color bowlAColor = Color.blue;
    [SerializeField] private Color bowlBColor = Color.red;

    [Header("Food Image")]
    [SerializeField] private Image foodImage;
    [SerializeField] private Sprite[] foodSprites;
    [Header("Empty Bowl")]
    [SerializeField] private Sprite emptyBowlImage;
    private bool isEmpty= false;
    public event Action<BowlVisual> OnFoodFinished;
    public bool IsEmpty => isEmpty;
    public void SetBowlType(bool isBowlA)
    {
        if (isBowlA)
        {
            foodImage.color = bowlAColor;
        }
        else
        {
            foodImage.color = bowlBColor;
        }
    }

    public void SetRandomFood()
    {
        if (foodSprites == null || foodSprites.Length == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, foodSprites.Length);
        foodImage.sprite = foodSprites[randomIndex];
    }
    public void SetEmptyBowl()
    {
        isEmpty = true;
        foodImage.sprite = emptyBowlImage;
    }
    public void OnClickEat()
    {
        if (isEmpty)
        {
            return;
        }

        SetEmptyBowl();
    }
}