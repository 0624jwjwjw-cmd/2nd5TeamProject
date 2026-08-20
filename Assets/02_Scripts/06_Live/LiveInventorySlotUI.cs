using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LiveInventorySlotUI : MonoBehaviour, IBeginDragHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMPro.TMP_Text _amountText;

    private string _itemId;
    private int _amount;

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

        if (_amountText != null)
            _amountText.text = amount.ToString();
    }

    public void Clear()
    {
        _itemId = null;
        _amount = 0;

        if (_icon != null)
            _icon.sprite = null;

        if (_amountText != null)
            _amountText.text = string.Empty;
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