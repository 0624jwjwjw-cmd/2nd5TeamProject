using UnityEngine;

public class FoodBowl : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private BowlVisual bowlVisual;
    public bool IsBowlA { get; private set; }

    public void Initialize(bool isBowlA)
    {
        IsBowlA = isBowlA;

        bowlVisual.SetBowlType(IsBowlA);
        bowlVisual.SetRandomFood();
    }
}