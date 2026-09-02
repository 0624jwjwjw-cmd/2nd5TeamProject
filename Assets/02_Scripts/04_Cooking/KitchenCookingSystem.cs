using UnityEngine;

public class KitchenCookingSystem : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlotManager kitchenCookingSlotManager;
    [SerializeField] private KitchenCookResult cookResult;
    [SerializeField] private string burnedDishID = "BD_01";
    [SerializeField] private string trashDishID = "TD_01";

    private readonly DishMatchingService dishMatchingService = new DishMatchingService();

    public void StartCook()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (IsAllSlotsEmpty())
        { 
            return; 
        }

        string resultDishID = dishMatchingService.FindMatchingDish(kitchenCookingSlotManager.slots);

        if (resultDishID == null)
        {
            ShowResultAndAddToInventory(trashDishID, ItemType.Dish, false);
            SoundManager.Instance.PlaySFX(SFXType.CookLose);
        }
        else if (!ReciepeUnlockManager.Instance.IsUnlocked(resultDishID))
        {
            ShowResultAndAddToInventory(burnedDishID, ItemType.Dish, false);
            SoundManager.Instance.PlaySFX(SFXType.CookLose);
        }
        else if (Random.Range(0, 100) < KitchenUpgradeManager.Instance.CurrentData.SpecialFoodRate)
        {
            string specialDishID = dishMatchingService.FindMatchingSpecialDish(resultDishID);
            ShowResultAndAddToInventory(specialDishID, ItemType.SpecialDish, true);
            SoundManager.Instance.PlaySFX(SFXType.CookWin);
        }
        else
        {
            ShowResultAndAddToInventory(resultDishID, ItemType.Dish, false);
            SoundManager.Instance.PlaySFX(SFXType.CookWin);
        }

        ConsumeIngredients();
        kitchenCookingSlotManager.ClearSlots();
        kitchenCookingSlotManager.ClearSolidSlotUIs();
    }

    private bool IsAllSlotsEmpty()
    {
        foreach (KitchenCookSlotItem item in kitchenCookingSlotManager.slots)
        {
            if (item != null && item.ingredientID != null)
            {
                return false;
            }
        }
        return true;
    }

    private void ShowResultAndAddToInventory(string dishID, ItemType itemType, bool isSpecial)
    {
        cookResult.gameObject.SetActive(true);
        if (isSpecial)
        {
            cookResult.SetResultSpecialDishInfo(dishID);
        }
        else
        {
            cookResult.SetResultDishInfo(dishID);
        }
        InventoryManager.Instance.AddItem(dishID, 1, itemType);
    }

    private void ConsumeIngredients()
    {
        foreach (KitchenCookSlotItem slot in kitchenCookingSlotManager.slots)
        {
            InventoryManager.Instance.RemoveItem(slot.ingredientID, slot.count);
        }
    }
}
