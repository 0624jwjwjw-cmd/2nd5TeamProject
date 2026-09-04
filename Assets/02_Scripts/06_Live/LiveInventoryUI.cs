using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private LiveInventorySlotUI slotPrefab;
    [SerializeField] private int maxSlotCount = 30;

    private readonly List<LiveInventorySlotUI> slots = new();

    private ItemVisualRepository _itemVisualRepository;
    private GameDataRepository _gameDataRepository;

    private void Awake()
    {
        _itemVisualRepository = ItemVisualRepository.Instance;
        _gameDataRepository = GameDataRepository.Instance;

        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (_itemVisualRepository == null)
        {
            return;
        }

        if (_gameDataRepository == null)
        {
            return;
        }

        if (content == null)
        {
            return;
        }

        if (slotPrefab == null)
        {
            return;
        }

        CreateSlots();
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= HandleInventoryChanged;
        }
    }

    private void HandleInventoryChanged(InventoryChange _)
    {
        Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    private void CreateSlots()
    {
        slots.Clear();

        for (int i = 0; i < maxSlotCount; i++)
        {
            LiveInventorySlotUI slot = Instantiate(
                slotPrefab,
                content
            );

            slot.Clear();
            slots.Add(slot);
        }
    }

    public void Refresh()
    {
        if (InventoryManager.Instance == null)
            return;

        if (_itemVisualRepository == null ||
            _gameDataRepository == null)
        {
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Clear();
        }

        IReadOnlyList<InventorySlotData> inventorySlots =
            InventoryManager.Instance.Slots;

        int slotIndex = 0;

        for (int i = 0;
             i < inventorySlots.Count && slotIndex < slots.Count;
             i++)
        {
            InventorySlotData slotData = inventorySlots[i];

            if (slotData == null || slotData.Amount <= 0)
                continue;

            string itemId = slotData.ItemId;

            if (string.IsNullOrWhiteSpace(itemId))
                continue;

            if (!_gameDataRepository.TryGetDish(
                    itemId,
                    out DishData dishData) &&
                !_gameDataRepository.TryGetSpecialDish(
                    itemId,
                    out dishData))
            {
                continue;
            }

            if (!_itemVisualRepository.TryGetIcon(
                    itemId,
                    out Sprite icon))
            {
                continue;
            }

            slots[slotIndex].Setup(
                itemId,
                icon,
                dishData.DishName,
                slotData.Amount
            );

            slotIndex++;
        }
    }
}