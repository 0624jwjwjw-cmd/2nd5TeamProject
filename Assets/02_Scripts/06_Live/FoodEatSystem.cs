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
            LiveManager.Instance.OnLiveEnded += StopEating;
        }
    }

    private void OnDestroy()
    {
        if (LiveManager.Instance != null)
        {
            LiveManager.Instance.OnLiveStarted -= StartEating;
            LiveManager.Instance.OnLiveEnded -= StopEating;
        }
    }

    private void Update()
    {
        if (!_isEating)
        {
            return;
        }

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
        _eatIndex = 0;
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
        FoodPlace[] foodPlaces = _foodArea.FoodPlaces;

        if (_eatIndex >= foodPlaces.Length)
        {
            StopEating();
            return;
        }

        FoodPlace foodPlace = foodPlaces[_eatIndex];
        DishBase dish = foodPlace.PlacedDish;

        if (dish != null)
        {
            LiveManager.Instance.EatFood(dish);

            foodPlace.RemoveFood(dish);

            Destroy(dish.gameObject);
        }

        _eatIndex++;
    }
}