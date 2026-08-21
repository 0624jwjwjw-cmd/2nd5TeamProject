using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform content;
    [SerializeField] private LiveInventorySlotUI slotPrefab;
    [SerializeField] private int maxSlotCount = 30;

    private readonly List<LiveInventorySlotUI> slots = new();

    private ItemVisualRepository _itemVisualRepository;
    private GameDataRepository _gameDataRepository;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        _itemVisualRepository = ItemVisualRepository.Instance;
        _gameDataRepository = GameDataRepository.Instance;

        if (content == null)
        {
            Debug.LogError("[LiveInventoryUI] Content가 연결되지 않았습니다.");
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogError("[LiveInventoryUI] Slot Prefab이 연결되지 않았습니다.");
            return;
        }

        CreateSlots();
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

    private void CreateSlots()
    {
        slots.Clear();

        for (int i = 0; i < maxSlotCount; i++)
        {
            LiveInventorySlotUI slot = Instantiate(slotPrefab, content);
            slot.Clear();
            slots.Add(slot);
        }
    }

    public void Refresh()
    {
        if (inventoryManager == null)
            return;

        if (_itemVisualRepository == null)
            _itemVisualRepository = ItemVisualRepository.Instance;

        if (_gameDataRepository == null)
            _gameDataRepository = GameDataRepository.Instance;

        for (int i = 0; i < slots.Count; i++)
            slots[i].Clear();

        IReadOnlyList<InventorySlotData> inventorySlots =
            inventoryManager.Slots;

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

            if (_gameDataRepository == null)
                continue;

            if (!_gameDataRepository.TryGetDish(itemId, out DishData dishData) &&
                !_gameDataRepository.TryGetSpecialDish(itemId, out dishData))
            {
                continue;
            }

            if (_itemVisualRepository == null ||
                !_itemVisualRepository.TryGetIcon(itemId, out Sprite icon))
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