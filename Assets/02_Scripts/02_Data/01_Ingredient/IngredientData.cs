using UnityEngine;

[CreateAssetMenu(fileName = "IG_", menuName = "GameData/Ingredient")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string ingredientName;
    [SerializeField] private int price;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;
    public string ID => id;
    public string IngredientName => ingredientName;
    public int Price => price;
    public int Donation => donation;
    public int Subscribers => subscribers;

    public void SetData(string id, string ingredientName, int price, int donation, int subscribers)
    {
        this.id = id;
        this.ingredientName = ingredientName;
        this.price = price;
        this.donation = donation;
        this.subscribers = subscribers;
    }
}
