using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenInventorySlot : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlotManager kitchenCookingSlotManager;
    [SerializeField] private Image image;
    [SerializeField] private string slotName;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private int amount;
    [SerializeField] private TMP_Text amountText;

    [SerializeField] private Image amountBackGround;

    private string slotID;
    public void OnEnable()
    {
        ResetSlot();
    }
    public void SetSlot(InventorySlotData inventorySlotData)
    {
        gameObject.SetActive(true);
        slotID = inventorySlotData.ItemId;

        image.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        amountText.gameObject.SetActive(true);

        if (ItemVisualRepository.Instance.TryGetIcon(inventorySlotData.ItemId, out Sprite icon))
        {
            image.sprite = icon;
        }

        if (inventorySlotData.ItemType == ItemType.Ingredient)
        {
            if (GameDataRepository.Instance.TryGetIngredient(inventorySlotData.ItemId, out IngredientData ingredientData))
            {
                slotName = ingredientData.IngredientName;
            }
        }
        else if (inventorySlotData.ItemType == ItemType.Dish)
        {
            if (GameDataRepository.Instance.TryGetDish(inventorySlotData.ItemId, out DishData dishData))
            {
                slotName = dishData.DishName;
            }
        }
        else if (inventorySlotData.ItemType == ItemType.SpecialDish)
        {
            if (GameDataRepository.Instance.TryGetSpecialDish(inventorySlotData.ItemId, out DishData specialDishData))
            {
                slotName = specialDishData.DishName;
            }
        }

        nameText.text = slotName;
        amount = inventorySlotData.Amount;
        amountText.text = amount.ToString();
    }
    public void ResetSlot()
    {
        slotID = "";
        image.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        image.sprite = null;
        slotName = "";
        nameText.text = "";
        amount = 0;
        amountText.text = "";
    }
    public void ClearSlot()
    {
        ResetSlot();
        gameObject.SetActive(false);
    }
    public void OnClickSlot()
    {
        kitchenCookingSlotManager.AddIngredient(slotID);
    }
}