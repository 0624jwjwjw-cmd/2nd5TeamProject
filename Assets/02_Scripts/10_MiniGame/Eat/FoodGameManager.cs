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

    private void Start()
    {
        StartFoodGame();
    }
    private void Update()
    {
        if (!isFoodGamePlaying)
        {
            return;
        }

        if (!miniGameManager.IsMiniGamePlaying)
        {
            StopFoodGame();
        }
    }
    public void StartFoodGame()
    {
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
        foodBowls.Remove(bowl);
        emptyBowls.Add(bowl);

        MoveNextFood();
    }

    private void MoveNextFood()
    {
        if (foodBowls.Count == 0)
        {
            FoodBowl bowl = emptyBowls[^1];
            foodMoveManager.MoveFoodToEmptyPlate(bowl.GetComponent<RectTransform>());
            StartWashingPhase();
            return;
        }
        if(emptyBowls.Count == 0)
        {
            FoodBowl nextFoodd = foodBowls[^1];
            foodMoveManager.MoveFoodToEatZone(nextFoodd.GetComponent<RectTransform>());
            return;
        }
        FoodBowl emptyBowl = emptyBowls[^1];
        FoodBowl nextFood = foodBowls[^1];
        foodMoveManager.MoveFoodToEmptyPlate(emptyBowl.GetComponent<RectTransform>());
        foodMoveManager.MoveFoodToEatZone(nextFood.GetComponent<RectTransform>());
    }

    private void StartWashingPhase()
    {
        currentPhase = GamePhase.Washing;
        MoveNextEmptyBowl();
    }

    private void MoveNextEmptyBowl()
    {
        if (emptyBowls.Count == 0)
        {
            StartNewRound();
            return;
        }
        FoodBowl bowl = emptyBowls[0];
        foodMoveManager.MoveEmptyPlateToEatZone(bowl.GetComponent<RectTransform>());
    }

    // 教农措俊辑 窍唱 贸府凳
    public void OnBowlWashed(FoodBowl bowl)
    {
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
        isFoodGamePlaying = false;
        currentPhase = GamePhase.Eating;

        foodBowls.Clear();
        emptyBowls.Clear();

        bowlSpawner.ClearBowls();
    }
}