using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform content;

    [Header("Dish Prefabs")]
    [SerializeField] private DishBase[] dishBases;

    private readonly List<LiveInventorySlotUI> slots = new();

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        if (content == null)
        {
            Debug.LogError("[LiveInventoryUI] Content가 연결되지 않았습니다.");
            return;
        }

        slots.Clear();
        slots.AddRange(
            content.GetComponentsInChildren<LiveInventorySlotUI>(true)
        );
    }

    private void OnEnable()
    {
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (inventoryManager == null)
            return;

        for (int i = 0; i < slots.Count; i++)
            slots[i].Clear();

        IReadOnlyList<InventorySlotData> inventorySlots =
            inventoryManager.Slots;

        for (int i = 0; i < inventorySlots.Count && i < slots.Count; i++)
        {
            InventorySlotData slotData = inventorySlots[i];

            if (slotData == null || slotData.Amount <= 0)
                continue;

            DishBase dishBase = FindDishBase(slotData.ItemId);

            if (dishBase == null)
            {
                Debug.LogWarning(
                    $"[LiveInventoryUI] DishBase를 찾을 수 없습니다. ID: {slotData.ItemId}"
                );
                continue;
            }

            slots[i].Setup(
                slotData.ItemId,
                dishBase,
                dishBase.DishName,
                slotData.Amount
            );
        }
    }

    private DishBase FindDishBase(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        for (int i = 0; i < dishBases.Length; i++)
        {
            DishBase dish = dishBases[i];

            if (dish == null)
                continue;

            if (dish.ID == itemId)
                return dish;

            if (dish.Data != null && dish.Data.ID == itemId)
                return dish;
        }

        return null;
    }
}