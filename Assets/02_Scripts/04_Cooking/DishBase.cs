using UnityEngine;

public class DishBase : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DishData data;
    [SerializeField] private string id;
    [SerializeField] private string dishName;
    [SerializeField] private string reciepeGrade;
    [SerializeField] private int cost;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;
    [SerializeField] private DishMaterial[] materials;
    [SerializeField] private string info;
    [Header("")]
    [SerializeField] public bool isUnlocked = false;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public DishData Data => data;
    public string ID => id;
    public string DishName => dishName;
    public string ReciepeGrade => reciepeGrade;
    public int Cost => cost;
    public int Donation => donation;
    public int Subscribers => subscribers;
    public DishMaterial[] Materials => materials;
    public string Info => info;

    private void Awake()
    {
        if (data == null)
        {
            return;
        }
        Initialize(data);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Initialize(DishData data)
    {
        id = data.ID;
        dishName = data.DishName;
        reciepeGrade = data.ReciepeGrade;
        cost = data.Cost;
        donation = data.Donation;
        subscribers = data.Subscribers;
        materials = data.Materials;
        info = data.Info;
    }
    public void Unlock()
    {
        isUnlocked = true;
    }
}
