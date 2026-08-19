using UnityEngine;

public class FoodPlaceController : MonoBehaviour
{
    public bool TryPlaceFood(
        FoodPlace foodPlace,
        LiveInventorySlotUI slotUI)
    {
        if (foodPlace == null || slotUI == null)
            return false;

<<<<<<< HEAD
        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        return foodPlace.TryPlace(itemId);
=======
        return foodPlace.TryPlaceFromInventory(slotUI);
>>>>>>> parent of 918d069 (/fix 음식 시스템 ItemId 기반으로 변경)
    }
}