using UnityEngine;

public class FoodPlaceController : MonoBehaviour
{
    public bool TryPlaceFood(FoodPlace foodPlace, string itemId)
    {
        if (foodPlace == null)
            return false;

        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        return foodPlace.TryPlace(itemId);
    }
}