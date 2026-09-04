using UnityEngine;

public class IngredientBase : MonoBehaviour
{
    [SerializeField] private IngredientData data;
    [SerializeField] private string id;
    [SerializeField] private string ingredientName;
    [SerializeField] private int price;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;

    public SpriteRenderer spriteRenderer;
    public IngredientData Data => data;
    public string ID => id;
    public string IngredientName => ingredientName;
    public int Price => price;
    public int Donation => donation;
    public int Subscribers => subscribers;


    private void Awake()
    {
        if(data == null)
        {
            return;
        }
        Initialize(data);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnValidate()
    {
        Initialize(data);
        spriteRenderer = GetComponent<SpriteRenderer>();
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
