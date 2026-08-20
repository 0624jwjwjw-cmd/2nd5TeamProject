using UnityEngine;
using UnityEngine.EventSystems;

public class FoodPlaceDropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private FoodPlace _foodPlace;
    [SerializeField] private FoodPlaceController _controller;
    [SerializeField] private FoodArea _foodArea;

    private void Awake()
    {
        if (_foodPlace == null)
            _foodPlace = GetComponent<FoodPlace>();

        if (_controller == null)
            _controller = GetComponentInParent<FoodPlaceController>();

        if (_foodArea == null)
            _foodArea = GetComponentInParent<FoodArea>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_foodPlace == null)
            return;

        if (_controller == null)
            return;

        if (FoodDrag.Instance == null)
            return;

        if (!FoodDrag.Instance.TryGetItemId(out string itemId))
            return;

        if (!_controller.TryPlaceFood(_foodPlace, itemId))
            return;

        FoodDrag.Instance.EndDrag();

        if (_foodArea != null)
            _foodArea.CheckFoodPlaces();
    }
}