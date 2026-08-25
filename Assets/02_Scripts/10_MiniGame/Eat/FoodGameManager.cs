using System.Collections.Generic;
using UnityEngine;

public class FoodGameManager : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] private BowlSpawner bowlSpawner;

    [Header("Move Manager")]
    [SerializeField] private FoodMoveManager foodMoveManager;

    [Header("Food")]
    [SerializeField] private List<FoodBowl> bowls;

    private int currentFoodIndex;

    private void Start()
    {
        StartFoodGame();
    }

    public void StartFoodGame()
    {
        currentFoodIndex = 0;

        // 그릇 생성
        bowlSpawner.SpawnBowls();

        // 생성된 그릇 가져오기
        bowls = bowlSpawner.GetBowls();

        // 첫 번째 음식 이동
        MoveNextFood();
    }

    private void MoveNextFood()
    {
        if (currentFoodIndex >= bowls.Count)
        {
            return;
        }

        FoodBowl bowl = bowls[currentFoodIndex];

        foodMoveManager.MoveFoodToEatZone(
            bowl.GetComponent<RectTransform>()
        );
    }

    private void OnFoodFinished(FoodBowl bowl)
    {
        // 먹은 음식 → 빈그릇 Zone
        foodMoveManager.MoveFoodToEmptyPlate(
            bowl.GetComponent<RectTransform>()
        );

        currentFoodIndex++;

        // 다음 음식 → 먹는 곳
        MoveNextFood();
    }
}