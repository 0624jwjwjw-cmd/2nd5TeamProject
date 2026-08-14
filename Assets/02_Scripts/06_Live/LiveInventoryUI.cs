using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform content;

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

            Sprite icon = null;

            if (ItemVisualRepository.Instance != null)
            {
                ItemVisualRepository.Instance.TryGetIcon(
                    slotData.ItemId,
                    out icon
                );
            }

            string itemName = slotData.ItemId;

            if (GameDataRepository.Instance != null)
            {
                if (GameDataRepository.Instance.TryGetDish(
                    slotData.ItemId,
                    out DishData dishData))
                {
                    itemName = dishData.DishName;
                }
            }

            slots[i].Setup(
                slotData.ItemId,
                icon,
                itemName,
                slotData.Amount
            );
        }
    }
}