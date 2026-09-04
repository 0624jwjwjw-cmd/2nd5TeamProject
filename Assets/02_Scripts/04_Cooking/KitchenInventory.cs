using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KitchenInventory : MonoBehaviour
{
    [SerializeField] private Image wholeButton;
    [SerializeField] private Image ingredientButton;
    [SerializeField] private Image dishButton;

    [SerializeField] private KitchenInventorySlot slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Transform poolRoot;

    [SerializeField] private Color selectedButtonColor = new Color(254, 195, 19);
    [SerializeField] private Color unSelectedButtonColor = new Color(252, 232, 204);

    private InventoryViewType currentView;
    private ComponentPool<KitchenInventorySlot> slotPool;
    private readonly List<KitchenInventorySlot> activeSlots = new();

    [SerializeField] private KitchenCookingSlotManager kitchenCookingSlotManager;
    private void Awake()
    {
        slotPool = new ComponentPool<KitchenInventorySlot>(slotPrefab, poolRoot);
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

        ReleaseAllSlots();

        for (int i = 0; i < InventoryManager.Instance.SlotCount; i++)
        {
            InventorySlotData slotData = InventoryManager.Instance.Slots[i];
            if (MatchesView(slotData, viewType))
            {
                KitchenInventorySlot slot = slotPool.Get(slotParent);
                slot.SetCookingSlotManager(kitchenCookingSlotManager);
                slot.SetSlot(slotData);
                activeSlots.Add(slot);
            }
        }
        UpdateButtonColor(currentView);
    }
    private void ReleaseAllSlots()
    {
        foreach (KitchenInventorySlot slot in activeSlots)
        {
            slot.ClearSlot();
            slotPool.Release(slot);
        }
        activeSlots.Clear();
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
    private void UpdateButtonColor(InventoryViewType viewType)
    {
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
}