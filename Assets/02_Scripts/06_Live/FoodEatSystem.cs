using UnityEngine;

public class FoodEatSystem : MonoBehaviour
{
    [SerializeField] private FoodArea _foodArea;
    [SerializeField] private float _eatInterval = 3f;

    private float _timer;
    private int _eatIndex;
    private bool _isEating;

    private void Start()
    {
        if (LiveManager.Instance != null)
        {
            LiveManager.Instance.OnLiveStarted += StartEating;
            LiveManager.Instance.OnLiveStopped += StopEating;
            LiveManager.Instance.OnLiveEnded += StopEating;
        }
    }

    private void OnDestroy()
    {
        if (LiveManager.Instance != null)
        {
            LiveManager.Instance.OnLiveStarted -= StartEating;
            LiveManager.Instance.OnLiveStopped -= StopEating;
            LiveManager.Instance.OnLiveEnded -= StopEating;
        }
    }

    private void Update()
    {
        if (!_isEating)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _eatInterval)
        {
            _timer = 0f;
            EatNextFood();
        }
    }

    private void StartEating()
    {
        _timer = 0f;
        _isEating = true;

        Debug.Log("음식 먹기 시작");
    }

    private void StopEating()
    {
        _isEating = false;
        _timer = 0f;
    }

    private void EatNextFood()
    {
        if (_foodArea == null)
        {
            StopEating();
            return;
        }

        FoodPlace[] foodPlaces = _foodArea.FoodPlaces;

        if (foodPlaces == null || foodPlaces.Length == 0)
        {
            StopEating();
            return;
        }

        while (_eatIndex < foodPlaces.Length)
        {
            FoodPlace foodPlace = foodPlaces[_eatIndex];
            _eatIndex++;

            if (foodPlace == null || !foodPlace.IsFilled)
                continue;

            DishBase dish = foodPlace.DishBase;

            if (dish == null)
                continue;

            LiveManager.Instance.EatFood(dish);
            foodPlace.RemoveFood();

            return;
        }

        StopEating();
    }
}