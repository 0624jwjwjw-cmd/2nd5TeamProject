using UnityEngine;
using UnityEngine.EventSystems;

public class FoodPlaceDropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private FoodPlace _foodPlace;
    [SerializeField] private FoodPlaceController _controller;

    private void Awake()
    {
        if (_foodPlace == null)
            _foodPlace = GetComponent<FoodPlace>();

        if (_controller == null)
            _controller = GetComponentInParent<FoodPlaceController>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_foodPlace == null)
            return;

        if (FoodDrag.Instance == null)
            return;

        if (!FoodDrag.Instance.TryGetItemId(out string itemId))
            return;

        if (_controller == null)
        {
            Debug.LogError("[FoodPlaceDropHandler] FoodPlaceController가 없습니다.");
            return;
        }

        if (_controller.TryPlaceFood(_foodPlace, itemId))
            FoodDrag.Instance.EndDrag();
    }
}