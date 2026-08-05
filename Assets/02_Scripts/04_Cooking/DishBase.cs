using UnityEngine;

public class DishBase : MonoBehaviour
{
    [SerializeField] private DishData data;
    [SerializeField] private string id;
    [SerializeField] private string dishName;
    [SerializeField] private string reciepeGrade;
    [SerializeField] private int cost;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;
    [SerializeField] private DishMaterial[] materials;
    [SerializeField] private string info;

    private void Awake()
    {
        if (data == null)
        {
            return;
        }
        Initialize(data);
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
}
