using System;
using UnityEngine;

public class FoodBowl : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private BowlVisual bowlVisual;
    public bool IsBowlA { get; private set; }
    public bool IsEmpty { get; private set; }
    public event Action<FoodBowl> OnFoodFinished;
    public void Initialize(bool isBowlA)
    {
        IsBowlA = isBowlA;
        IsEmpty = false;

        bowlVisual.SetFoodVisual(IsBowlA);
    }
    public void SetFoodFill(float amount)
    {
        bowlVisual.SetFoodFill(amount);
    }

    public void SetEmpty()
    {
        IsEmpty = true;
        bowlVisual.SetEmptyVisual(IsBowlA);
        OnFoodFinished?.Invoke(this);
    }
}