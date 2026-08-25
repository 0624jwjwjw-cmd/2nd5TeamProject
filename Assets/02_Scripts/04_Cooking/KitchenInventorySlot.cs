using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenInventorySlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private string slotName;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private int amount;
    [SerializeField] private TMP_Text amountText;

    [SerializeField] private CookSlotManager cookSlotManager;

    public void SetSlot(InventorySlotData inventorySlotData)
    {
        if (ItemVisualRepository.Instance.TryGetIcon(inventorySlotData.ItemId, out Sprite icon))
        {
            image.sprite = icon;
        }
        else
        {
            return;
        }

        if (inventorySlotData.ItemType == ItemType.Ingredient)
        {
            if (GameDataRepository.Instance.TryGetIngredient(inventorySlotData.ItemId, out IngredientData ingredientData))
            {
                slotName = ingredientData.IngredientName;
            }
            else
            {
                return;
            }
        }
        else if (inventorySlotData.ItemType == ItemType.Dish || inventorySlotData.ItemType == ItemType.SpecialDish)
        {
            if(GameDataRepository.Instance.TryGetDish(inventorySlotData.ItemId, out DishData dishData))
            {
                slotName = dishData.DishName;
            }
            else
            {
                return;
            }
        }

        nameText.text = slotName;
        amount = inventorySlotData.Amount;
        amountText.text = amount.ToString();
    }
    public void ClearSlot()
    {
        image.sprite = null;
        slotName = null;
        nameText.text = null;
        amount = 0;
        amountText.text = null;
    }
    public void OnClickSlot()
    {

    }
}