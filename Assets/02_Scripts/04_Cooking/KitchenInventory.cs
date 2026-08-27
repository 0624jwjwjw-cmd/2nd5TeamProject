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
        //InventoryManager.Instance.OnInventoryChanged += RefreshView;
        StartCoroutine(WaitforMangaer());
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
        currentView = InventoryViewType.Whole;
        for(int i=0; i<InventoryManager.Instance.SlotCount;i++)
        {
            slots[i].SetSlot(InventoryManager.Instance.Slots[i]);
        }
        for (int i = InventoryManager.Instance.SlotCount; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        wholeButton.color = selectedButtonColor;
        ingredientButton.color = unSelectedButtonColor;
        dishButton.color = unSelectedButtonColor;
    }
    public void SetIngredient()
    {
        currentView = InventoryViewType.Ingredient;
        int slotIndex = 0;

        for (int i = 0; i < InventoryManager.Instance.SlotCount; i++)
        {
            InventorySlotData slotData = InventoryManager.Instance.Slots[i];
            if (slotData != null && slotData.ItemType == ItemType.Ingredient)
            {
                if (slotIndex < slots.Length)
                {
                    slots[slotIndex].SetSlot(InventoryManager.Instance.Slots[i]);
                    slotIndex++;
                }
            }
        }

        for (int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        wholeButton.color = unSelectedButtonColor;
        ingredientButton.color = selectedButtonColor;
        dishButton.color = unSelectedButtonColor;
    }
    public void SetDish()
    {
        currentView = InventoryViewType.Dish;
        int slotIndex = 0;

        for(int i=0; i<InventoryManager.Instance.SlotCount; i++)
        {
            InventorySlotData slotData = InventoryManager.Instance.Slots[i];
            if (slotData == null) return;
            if (slotData.ItemType == ItemType.Dish || slotData.ItemType == ItemType.SpecialDish)
            {
                if (slotIndex < slots.Length)
                {
                    slots[slotIndex].SetSlot(InventoryManager.Instance.Slots[i]);
                    slotIndex++;
                }
            }
        }
        for(int i= slotIndex; i<slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        wholeButton.color = unSelectedButtonColor;
        ingredientButton.color = unSelectedButtonColor;
        dishButton.color = selectedButtonColor;
    }
    private void RefreshView()
    {
        switch(currentView)
        {
            case InventoryViewType.Whole:
                SetWhole();
                break;
            case InventoryViewType.Ingredient:
                SetIngredient();
                break;
            case InventoryViewType.Dish:
                SetDish();
                break;
        }
    }
    ////////////////////////////
    private IEnumerator WaitforMangaer()
    {
        while (InventoryManager.Instance == null)
        {
            yield return null;
        }
        InventoryManager.Instance.OnInventoryChanged += RefreshView;
    }
}
