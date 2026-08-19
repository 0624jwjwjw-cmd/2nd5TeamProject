using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LiveInventorySlotUI : MonoBehaviour, IBeginDragHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _amountText;

    private string _itemId;

    public string ItemId => _itemId;

    public void Setup(
        string itemId,
        Sprite icon,
        int amount)
    {
        _itemId = itemId;

        if (_iconImage != null)
            _iconImage.sprite = icon;

        if (_amountText != null)
            _amountText.text = amount.ToString();

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        _itemId = null;

        if (_iconImage != null)
            _iconImage.sprite = null;

        if (_amountText != null)
            _amountText.text = string.Empty;

        gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(_itemId))
            return;

        if (FoodDrag.Instance == null)
            return;

        FoodDrag.Instance.BeginDrag(_itemId);
    }
}