using UnityEngine;

public class FoodMoveManager : MonoBehaviour
{
    [Header("Mover")]
    [SerializeField] private FoodMover foodMover;

    [Header("Move Destination")]
    [SerializeField] private RectTransform eatPanel;
    [SerializeField] private RectTransform emptyPlatePanel;
    [SerializeField] private RectTransform foodPanel;

    // À½½Ä ¡æ ¸Ô´Â °÷
    public void MoveFoodToEatZone(RectTransform food)
    {
        foodMover.Move(food, eatPanel);
    }

    // ¸ÔÀº À½½Ä ¡æ ºó Á¢½Ã Zone
    public void MoveFoodToEmptyPlate(RectTransform food)
    {
        foodMover.Move(food, emptyPlatePanel);
    }

    // ºó Á¢½Ã ¡æ ¸Ô´Â °÷
    public void MoveEmptyPlateToEatZone(RectTransform emptyPlate)
    {
        foodMover.Move(emptyPlate, eatPanel);
    }
}