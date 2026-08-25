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

    [SerializeField] private Image amountBackGround;
    public void OnEnable()
    {
        ClearSlot();
    }
    public void SetSlot(InventorySlotData inventorySlotData)
    {
        image.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        amountText.gameObject.SetActive(true);

        if (ItemVisualRepository.Instance.TryGetIcon(inventorySlotData.ItemId, out Sprite icon))
        {
            image.sprite = icon;
        }
        else
        {
            Debug.Log("아이콘 없는디");
        }

        if (inventorySlotData.ItemType == ItemType.Ingredient)
        {
            if (GameDataRepository.Instance.TryGetIngredient(inventorySlotData.ItemId, out IngredientData ingredientData))
            {
                slotName = ingredientData.IngredientName;
            }
        }
        else if (inventorySlotData.ItemType == ItemType.Dish || inventorySlotData.ItemType == ItemType.SpecialDish)
        {
            if (GameDataRepository.Instance.TryGetDish(inventorySlotData.ItemId, out DishData dishData))
            {
                slotName = dishData.DishName;
            }
        }
        //else if (inventorySlotData.ItemType == ItemType.SpecialDish)
        //{
        //    if (GameDataRepository.Instance.TryGetSpecialDish(inventorySlotData.ItemId, out DishData specialDishData))
        //    {
        //        slotName = specialDishData.DishName;
        //    }
        //}

        nameText.text = slotName;
        amount = inventorySlotData.Amount;
        amountText.text = amount.ToString();
    }
    public void ClearSlot()
    {
        image.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        image.sprite = null;
        slotName = "";
        nameText.text = "";
        amount = 0;
        amountText.text = "";
    }
}