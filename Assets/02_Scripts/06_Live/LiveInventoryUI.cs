using System.Collections.Generic;
using UnityEngine;

public class LiveInventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform content;
<<<<<<< HEAD
=======

    [Header("Dish Prefabs")]
    [SerializeField] private DishBase[] dishBases;
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)

    private readonly List<LiveInventorySlotUI> slots = new();

    private ItemVisualRepository _itemVisualRepository;
    private GameDataRepository _gameDataRepository;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

<<<<<<< HEAD
        _itemVisualRepository = ItemVisualRepository.Instance;
        _gameDataRepository = GameDataRepository.Instance;

=======
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
        if (content == null)
        {
            Debug.LogError("[LiveInventoryUI] Content°¡ ¿¬°áµÇÁö ¾Ê¾Ò½À´Ï´Ù.");
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

        if (_itemVisualRepository == null)
            _itemVisualRepository = ItemVisualRepository.Instance;

        if (_gameDataRepository == null)
            _gameDataRepository = GameDataRepository.Instance;

        for (int i = 0; i < slots.Count; i++)
            slots[i].Clear();

        IReadOnlyList<InventorySlotData> inventorySlots =
            inventoryManager.Slots;

        int slotIndex = 0;

        for (int i = 0; i < inventorySlots.Count && slotIndex < slots.Count; i++)
        {
            InventorySlotData slotData = inventorySlots[i];

            if (slotData == null || slotData.Amount <= 0)
                continue;

            DishBase dishBase = FindDishBase(slotData.ItemId);

<<<<<<< HEAD
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
=======
            if (dishBase == null)
            {
                Debug.LogWarning(
                    $"[LiveInventoryUI] DishBase¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù. ID: {slotData.ItemId}"
                );
                continue;
            }

            slots[i].Setup(
                slotData.ItemId,
                dishBase,
                dishBase.DishName,
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
                slotData.Amount
            );

            slotIndex++;
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