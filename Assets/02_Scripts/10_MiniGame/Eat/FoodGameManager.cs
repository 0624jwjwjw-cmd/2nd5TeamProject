using System.Collections.Generic;
using UnityEngine;

public class FoodGameManager : MonoBehaviour
{
    [Header("Mini Game Manager")]
    [SerializeField] private MiniGameManager miniGameManager;

    [Header("Spawner")]
    [SerializeField] private BowlSpawner bowlSpawner;

    [Header("Move Manager")]
    [SerializeField] private FoodMoveManager foodMoveManager;

    [Header("Panels")]
    [SerializeField] private RectTransform eatPanel;

    private List<FoodBowl> foodBowls = new List<FoodBowl>();
    private List<FoodBowl> emptyBowls = new List<FoodBowl>();

    private bool isFoodGamePlaying;

    private enum GamePhase
    {
        Eating,
        Washing
    }

    private GamePhase currentPhase;

    private void OnEnable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged += HandleGameState;
        }
    }

    private void OnDisable()
    {
        if (miniGameManager != null)
        {
            miniGameManager.OnMiniGamePlayingChanged -= HandleGameState;
        }

        StopFoodGame();
    }

    private void HandleGameState(bool isPlaying)
    {
        if (isPlaying)
        {
            StartFoodGame();
        }
        else
        {
            StopFoodGame();
        }
    }

    public void StartFoodGame()
    {
        if (isFoodGamePlaying)
        {
            return;
        }

        isFoodGamePlaying = true;
        currentPhase = GamePhase.Eating;

        foodBowls.Clear();
        emptyBowls.Clear();

        bowlSpawner.SpawnBowls();

        foreach (FoodBowl bowl in bowlSpawner.GetBowls())
        {
            foodBowls.Add(bowl);
            bowl.OnFoodFinished += OnFoodFinished;
        }

        MoveNextFood();
    }

    private void OnFoodFinished(FoodBowl bowl)
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        foodBowls.Remove(bowl);
        emptyBowls.Add(bowl);

        MoveNextFood();
    }

    private void MoveNextFood()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        // 모든 음식을 먹었으면
        if (foodBowls.Count == 0)
        {
            FoodBowl lastEmptyBowl = emptyBowls[^1];

            foodMoveManager.MoveFoodToEmptyPlate(
                lastEmptyBowl.GetComponent<RectTransform>()
            );

            StartWashingPhase();

            return;
        }

        // 빈 그릇이 없으면
        if (emptyBowls.Count == 0)
        {
            FoodBowl firstFood = foodBowls[^1];

            foodMoveManager.MoveFoodToEatZone(
                firstFood.GetComponent<RectTransform>()
            );

            return;
        }

        // 빈 그릇과 음식이 모두 있으면
        FoodBowl emptyBowl = emptyBowls[^1];
        FoodBowl foodBowl = foodBowls[^1];

        foodMoveManager.MoveFoodToEmptyPlate(
            emptyBowl.GetComponent<RectTransform>()
        );

        foodMoveManager.MoveFoodToEatZone(
            foodBowl.GetComponent<RectTransform>()
        );
    }

    private void StartWashingPhase()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        currentPhase = GamePhase.Washing;

        MoveNextEmptyBowl();
    }

    private void MoveNextEmptyBowl()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        if (emptyBowls.Count == 0)
        {
            StartNewRound();
            return;
        }

        FoodBowl bowl = emptyBowls[0];

        foodMoveManager.MoveEmptyPlateToEatZone(
            bowl.GetComponent<RectTransform>()
        );
    }

    public void OnBowlWashed(FoodBowl bowl)
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        if (currentPhase != GamePhase.Washing)
        {
            return;
        }

        if (bowl == null)
        {
            return;
        }

        emptyBowls.Remove(bowl);

        MoveNextEmptyBowl();
    }

    private void StartNewRound()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        currentPhase = GamePhase.Eating;

        foodBowls.Clear();
        emptyBowls.Clear();

        bowlSpawner.SpawnBowls();

        foreach (FoodBowl bowl in bowlSpawner.GetBowls())
        {
            foodBowls.Add(bowl);
            bowl.OnFoodFinished += OnFoodFinished;
        }

        MoveNextFood();
    }

    public void StopFoodGame()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        isFoodGamePlaying = false;
        currentPhase = GamePhase.Eating;

        foodBowls.Clear();
        emptyBowls.Clear();

        bowlSpawner.ClearBowls();
    }
}