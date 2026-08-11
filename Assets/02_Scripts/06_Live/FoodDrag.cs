using UnityEngine;

public class FoodDrag : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isDragging;
    private Vector3 _offset;

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
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = Mathf.Abs(
            _mainCamera.transform.position.z
        );

        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
}