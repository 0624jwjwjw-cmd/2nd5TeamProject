using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform content;
    [SerializeField] private ItemVisualRepository itemVisualRepository;

    private readonly List<LiveInventorySlotUI> slots = new();

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        if (itemVisualRepository == null)
            itemVisualRepository = ItemVisualRepository.Instance;

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

            string itemId = slotData.ItemId;

            if (string.IsNullOrEmpty(itemId))
                continue;

            if (itemVisualRepository == null ||
                !itemVisualRepository.TryGetIcon(itemId, out Sprite icon))
            {
                Debug.LogWarning(
                    $"[LiveInventoryUI] 아이콘을 찾을 수 없습니다. ID: {itemId}"
                );
                continue;
            }

            slots[i].Setup(
                itemId,
                icon,
                slotData.Amount
            );
        }
    }
}