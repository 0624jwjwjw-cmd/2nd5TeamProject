using UnityEngine;

public class FoodPlaceController : MonoBehaviour
{
    public bool TryPlaceFood(FoodPlace foodPlace, string itemId)
    {
        if (foodPlace == null)
            return false;

        if (string.IsNullOrEmpty(itemId))
            return false;

        return foodPlace.TryPlace(itemId);
    }
}