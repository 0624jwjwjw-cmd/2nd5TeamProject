using System.Collections.Generic;
using UnityEngine;

public class FoodGameManager : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] private BowlSpawner bowlSpawner;

    [Header("Move Manager")]
    [SerializeField] private FoodMoveManager foodMoveManager;

    [Header("Panels")]
    [SerializeField] private RectTransform eatPanel;

    private List<FoodBowl> bowls;

    private int currentFoodIndex;
    private int washedBowlCount;

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

    public void StartFoodGame()
    {
        currentPhase = GamePhase.Eating;

        currentFoodIndex = 0;
        washedBowlCount = 0;

        bowlSpawner.SpawnBowls();

        bowls = bowlSpawner.GetBowls();

        foreach (FoodBowl bowl in bowls)
        {
            bowl.OnFoodFinished += OnFoodFinished;
        }

        MoveNextFood();
    }

    private void OnFoodFinished(FoodBowl bowl)
    {
        // ¸Ô¹æ ¡æ ºó±×¸© Á¤¸®
        foodMoveManager.MoveFoodToEmptyPlate(
            bowl.GetComponent<RectTransform>()
        );

        currentFoodIndex++;

        MoveNextFood();
    }

    private void MoveNextFood()
    {
        if (currentFoodIndex >= bowls.Count)
        {
            StartWashingPhase();
            return;
        }

        // ¾Æ·¡ÂÊ À½½ÄºÎÅÍ
        int bowlIndex = bowls.Count - 1 - currentFoodIndex;

        FoodBowl bowl = bowls[bowlIndex];

        foodMoveManager.MoveFoodToEatZone(
            bowl.GetComponent<RectTransform>()
        );
    }

    private void StartWashingPhase()
    {
        currentPhase = GamePhase.Washing;

        washedBowlCount = 0;

        MoveNextEmptyBowl();
    }

    private void MoveNextEmptyBowl()
    {
        foreach (FoodBowl bowl in bowls)
        {
            if (bowl == null)
            {
                continue;
            }

            if (!bowl.IsEmpty)
            {
                continue;
            }

            // ÇöÀç ¸Ô¹æ¿¡ ÀÖ´Â ºó±×¸©Àº Á¦¿Ü
            if (bowl.transform.parent == eatPanel)
            {
                continue;
            }

            foodMoveManager.MoveEmptyPlateToEatZone(
                bowl.GetComponent<RectTransform>()
            );

            return;
        }
    }

    // ½ÌÅ©´ë¿¡¼­ ÇÏ³ª Ã³¸®µÊ
    public void OnBowlWashed(FoodBowl bowl)
    {
        if (currentPhase != GamePhase.Washing)
        {
            return;
        }

        if (bowl == null || !bowl.IsEmpty)
        {
            return;
        }

        washedBowlCount++;

        if (washedBowlCount >= bowls.Count)
        {
            StartNewRound();
        }
        else
        {
            MoveNextEmptyBowl();
        }
    }

    private void StartNewRound()
    {
        currentPhase = GamePhase.Eating;

        currentFoodIndex = 0;
        washedBowlCount = 0;

        bowlSpawner.SpawnBowls();

        bowls = bowlSpawner.GetBowls();

        foreach (FoodBowl bowl in bowls)
        {
            bowl.OnFoodFinished += OnFoodFinished;
        }

        MoveNextFood();
    }
}