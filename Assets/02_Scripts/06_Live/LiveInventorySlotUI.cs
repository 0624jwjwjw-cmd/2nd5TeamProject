using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LiveInventorySlotUI : MonoBehaviour, IBeginDragHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private int _amount;
    [SerializeField] private string _itemId;

    public string ItemId => _itemId;
    public int Amount => _amount;

    public void Setup(
        string itemId,
        Sprite icon,
        string itemName,
        int amount)
    {
        _itemId = itemId;
        _amount = amount;

        if (_icon != null)
            _icon.sprite = icon;
    }

    public void Clear()
    {
        _itemId = null;
        _amount = 0;

        if (_icon != null)
            _icon.sprite = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrWhiteSpace(_itemId))
            return;

        if (FoodDrag.Instance == null)
            return;

        FoodDrag.Instance.BeginDrag(_itemId);
    }
}