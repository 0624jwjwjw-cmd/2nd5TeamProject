using UnityEngine;

public class FoodPlaceController : MonoBehaviour
{
    public bool TryPlaceFood(
        FoodPlace foodPlace,
        LiveInventorySlotUI slotUI)
    {
        if (foodPlace == null || slotUI == null)
            return false;

        return foodPlace.TryPlaceFromInventory(slotUI);
    }
}