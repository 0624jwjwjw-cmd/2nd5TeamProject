using UnityEngine;
using UnityEngine.UI;

public class FoodArea : MonoBehaviour
{
    [SerializeField] private FoodPlace[] _foodPlaces;
    public FoodPlace[] FoodPlaces => _foodPlaces;

    [SerializeField] private Button _startButton;

    private void Awake()
    {
        _startButton.interactable = false;
    }

    public void CheckFoodPlaces()
    {
        if (LiveManager.Instance != null && LiveManager.Instance.IsLive)
        {
            return;
        }

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