using UnityEngine;

public class FoodDrag : MonoBehaviour
{
    private Camera _mainCamera;
    private DishBase _dishBase;

    private bool _isDragging;
    private Vector3 _offset;

    private FoodPlace _currentFoodPlace;

    private SpriteRenderer _spriteRenderer;
    private int _originalSortingOrder;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _dishBase = GetComponent<DishBase>();

        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer != null)
        {
            _originalSortingOrder = _spriteRenderer.sortingOrder;
        }
    }

    private void Update()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        Vector3 pointerPosition = GetPointerWorldPosition();

        if (!_isDragging)
        {
            TryStartDrag(pointerPosition);
            return;
        }

        if (InputManager.Instance.IsDragging)
        {
            Vector3 newPosition = pointerPosition + _offset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            PlaceFood();
        }
    }

    private void TryStartDrag(Vector3 pointerPosition)
    {
        if (!InputManager.Instance.IsDragging)
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(pointerPosition);

        if (hit == null || hit.gameObject != gameObject)
        {
            return;
        }

        if (_currentFoodPlace != null)
        {
            _currentFoodPlace.RemoveFood(_dishBase);
            _currentFoodPlace = null;
        }

        _offset = transform.position - pointerPosition;
        _isDragging = true;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = 10;
        }
    }

    private void PlaceFood()
    {
        _isDragging = false;

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = _originalSortingOrder;
        }

        Collider2D[] colliders = Physics2D.OverlapPointAll(
            transform.position
        );

        foreach (Collider2D collider in colliders)
        {
            FoodPlace foodPlace = collider.GetComponent<FoodPlace>();

            if (foodPlace == null || foodPlace.IsFilled)
            {
                continue;
            }

            if (_dishBase == null)
            {
                return;
            }

            foodPlace.PlaceFood(_dishBase);
            _currentFoodPlace = foodPlace;

            return;
        }
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector3 position = InputManager.Instance.PointerPosition;

        position.z = Mathf.Abs(_mainCamera.transform.position.z);

        return _mainCamera.ScreenToWorldPoint(position);
    }
}