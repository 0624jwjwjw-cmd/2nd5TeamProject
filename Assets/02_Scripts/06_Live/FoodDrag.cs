using UnityEngine;

public class FoodDrag : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _offset;
    private bool _isDragging;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        _offset = transform.position - mousePosition;
        _isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!_isDragging)
        {
            return;
        }

        Vector3 mousePosition = GetMouseWorldPosition();
        transform.position = mousePosition + _offset;
    }

    private void OnMouseUp()
    {
        _isDragging = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            0.5f
        );

        foreach (Collider2D collider in colliders)
        {
            FoodPlace foodPlace = collider.GetComponent<FoodPlace>();

            if (foodPlace == null)
            {
                continue;
            }

            if (foodPlace.IsFilled)
            {
                continue;
            }

            DishBase dish = GetComponent<DishBase>();

            if (dish == null)
            {
                return;
            }

            foodPlace.PlaceFood(dish);
            return;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z);

        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
}