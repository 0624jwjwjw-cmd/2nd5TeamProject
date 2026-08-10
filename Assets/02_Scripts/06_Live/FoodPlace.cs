using UnityEngine;

public class FoodPlace : MonoBehaviour
{
    private DishBase placedDish;

    public bool IsFilled => placedDish != null;

    public void PlaceFood(DishBase dish)
    {
        if (IsFilled)
        {
            return;
        }

        placedDish = dish;

        dish.transform.position = transform.position;
    }
}