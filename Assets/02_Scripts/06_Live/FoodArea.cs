using UnityEngine;
using UnityEngine.UI;

public class FoodArea : MonoBehaviour
{
    [SerializeField] private FoodPlace[] _foodPlaces;
    [SerializeField] private Button _startButton;

    public FoodPlace[] FoodPlaces => _foodPlaces;

    private void Awake()
    {
        CheckFoodPlaces();
    }

    public void CheckFoodPlaces()
    {
        if (_startButton == null)
        {
            return;
        }

        if (LiveManager.Instance != null &&
            LiveManager.Instance.IsLive)
        {
            _startButton.interactable = false;
            return;
        }

        if (_foodPlaces == null || _foodPlaces.Length == 0)
        {
            _startButton.interactable = false;
            return;
        }

        foreach (FoodPlace foodPlace in _foodPlaces)
        {
            if (foodPlace == null || !foodPlace.IsFilled)
            {
                _startButton.interactable = false;
                return;
            }
        }

        _startButton.interactable = true;
    }
}