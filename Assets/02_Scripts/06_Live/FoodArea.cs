using UnityEngine;
using UnityEngine.UI;

public class FoodArea : MonoBehaviour
{
    [SerializeField] private FoodPlace[] _foodPlaces;
    [SerializeField] private Button _startButton;

    private void Awake()
    {
        _startButton.interactable = false;
    }

    public void CheckFoodPlaces()
    {
        foreach (FoodPlace foodPlace in _foodPlaces)
        {
            if (!foodPlace.IsFilled)
            {
                _startButton.interactable = false;
                return;
            }
        }

        _startButton.interactable = true;
    }
}