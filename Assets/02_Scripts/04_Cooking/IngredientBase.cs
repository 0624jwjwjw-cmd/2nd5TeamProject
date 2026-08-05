using UnityEngine;

public class IngredientBase : MonoBehaviour
{
    [SerializeField] private IngredientData data;
    [SerializeField] private string id;
    [SerializeField] private string ingredientName;
    [SerializeField] private int price;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;

    private void Awake()
    {
        if(data == null)
        {
            return;
        }
        Initialize(data);
    }
    private void Initialize(IngredientData data)
    {
        id = data.ID;
        ingredientName = data.IngredientName;
        price = data.Price;
        donation = data.Donation;
        subscribers = data.Subscribers;
    }

}
