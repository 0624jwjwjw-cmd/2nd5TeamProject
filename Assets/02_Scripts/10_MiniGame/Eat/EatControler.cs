using UnityEngine;
public class EatController : MonoBehaviour
{
    [SerializeField] private int maxEatCount = 10;

    private FoodBowl currentFood;
    private int eatCount;

    private void Update()
    {
        if (currentFood == null)
        {
            FoodBowl[] foods = GetComponentsInChildren<FoodBowl>();

            foreach (FoodBowl food in foods)
            {
                if (!food.IsEmpty)
                {
                    currentFood = food;
                    eatCount = 0;

                    Debug.Log("새로운 음식이 들어왔습니다.");
                    break;
                }
            }

        }
        if (InputManager.Instance.IsTap)
        {
            OnTouch();
        }
    }

    public void OnTouch()
    {
        if (currentFood == null || currentFood.IsEmpty)
        {
            return;
        }

        eatCount++;

        float fillAmount = 1f - (float)eatCount / maxEatCount;

        currentFood.SetFoodFill(fillAmount);

        if (eatCount >= maxEatCount)
        {
            currentFood.SetEmpty();
            currentFood = null;
        }
    }
}
