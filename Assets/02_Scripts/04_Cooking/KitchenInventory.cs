using NUnit.Framework.Constraints;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenInventory : MonoBehaviour
{
    [SerializeField] private Image wholeButton;
    [SerializeField] private Image ingredientButton;
    [SerializeField] private Image dishButton;
    [SerializeField] private KitchenInventorySlot[] slots;
    [SerializeField] private Color selectedButtonColor = new Color(254, 195, 19);
    [SerializeField] private Color unSelectedButtonColor = new Color(252, 232, 204);

    private InventoryViewType currentView;
    private void OnValidate()
    {
        slots = GetComponentsInChildren<KitchenInventorySlot>(true);
    }
    private void OnEnable()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshView;
    }
    private void OnDisable()
    {
        InventoryManager.Instance.OnInventoryChanged -= RefreshView;
    }
    private void Start()
    {
        SetWhole();
    }
    public void SetWhole()
    {
        SetData(InventoryViewType.Whole);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void SetIngredient()
    {
        SetData(InventoryViewType.Ingredient);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void SetDish()
    {
        SetData(InventoryViewType.Dish);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    private void SetData(InventoryViewType viewType)
    {
        currentView = viewType;
        int slotIndex = 0;
        for (int i = 0; i < InventoryManager.Instance.SlotCount; i++)
        {
            InventorySlotData slotData = InventoryManager.Instance.Slots[i];
            if (MatchesView(slotData, viewType))
            {
                if (slotIndex < slots.Length)
                {
                    slots[slotIndex].SetSlot(slotData);
                    slotIndex++;
                }
            }
        }
        for (int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        if (viewType == InventoryViewType.Whole)
        {
            wholeButton.color = selectedButtonColor;
            ingredientButton.color = unSelectedButtonColor;
            dishButton.color = unSelectedButtonColor;
        }
        else if (viewType == InventoryViewType.Ingredient)
        {
            wholeButton.color = unSelectedButtonColor;
            ingredientButton.color = selectedButtonColor;
            dishButton.color = unSelectedButtonColor;
        }
        else if (viewType == InventoryViewType.Dish)
        {
            wholeButton.color = unSelectedButtonColor;
            ingredientButton.color = unSelectedButtonColor;
            dishButton.color = selectedButtonColor;
        }
    }
    private void RefreshView()
    {
        switch (currentView)
        {
            case InventoryViewType.Whole:
                SetData(InventoryViewType.Whole);
                break;
            case InventoryViewType.Ingredient:
                SetData(InventoryViewType.Ingredient);
                break;
            case InventoryViewType.Dish:
                SetData(InventoryViewType.Dish);
                break;
        }
    }
    private bool MatchesView(InventorySlotData slotData, InventoryViewType viewType)
    {
        if (slotData == null)
        {
            return false;
        }

        if (viewType == InventoryViewType.Whole)
        {
            return true;
        }
        else if(viewType == InventoryViewType.Ingredient)
        {
            if(slotData.ItemType == ItemType.Ingredient)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(viewType == InventoryViewType.Dish)
        {
            if(slotData.ItemType == ItemType.Dish || slotData.ItemType == ItemType.SpecialDish)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}