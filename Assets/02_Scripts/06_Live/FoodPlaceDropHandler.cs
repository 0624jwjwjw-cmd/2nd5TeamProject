using UnityEngine;
using UnityEngine.EventSystems;

public class FoodPlaceDropHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private FoodPlace _foodPlace;
    [SerializeField] private FoodPlaceController _foodPlaceController;

    private void Awake()
    {
        if (_foodPlace == null)
            _foodPlace = GetComponent<FoodPlace>();

        if (_foodPlaceController == null)
            _foodPlaceController =
                FindFirstObjectByType<FoodPlaceController>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (FoodDrag.Instance == null)
            return;

        if (!FoodDrag.Instance.TryGetItemId(out string itemId))
            return;

        if (_foodPlaceController == null)
            return;

        if (_foodPlaceController.TryPlaceFood(_foodPlace, itemId))
        {
            FoodDrag.Instance.EndDrag();
        }
    }
}