using UnityEngine;

public class FoodPlace : MonoBehaviour
{
    [SerializeField] private FoodArea _foodArea;

    private DishBase _placedDish;

    public bool IsFilled => _placedDish != null;
    public DishBase PlacedDish => _placedDish;

    public void PlaceFood(DishBase dish)
    {
        if (IsFilled || dish == null)
        {
            return;
        }

        _placedDish = dish;
        dish.transform.position = transform.position;

        if (_foodArea != null)
        {
            _foodArea.CheckFoodPlaces();
        }
    }

    public void RemoveFood(DishBase dish)
    {
        if (_placedDish != dish)
        {
            return;
        }

        _placedDish = null;

        if (_foodArea != null)
        {
            _foodArea.CheckFoodPlaces();
        }
    }
}