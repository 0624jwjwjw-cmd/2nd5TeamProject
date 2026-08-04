using System;
using UnityEngine;

[Serializable]
public class DishMaterial
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private DishData dishData;
    [SerializeField] private int amount;
    public string GetID()
    {
        if(ingredientData != null)
        {
            return ingredientData.ID;
        }
        else if(dishData != null)
        {
            return dishData.ID;
        }
        else
        {
            return null;
        }
    }
    public DishMaterial() { }
    public DishMaterial(IngredientData ingredientData, DishData dishData, int amount)
    {
        if (ingredientData == null && dishData == null) return;
        this.ingredientData = ingredientData;
        this.dishData = dishData;
        this.amount = amount;
    }
    public string MaterialID => GetID();
    public IngredientData IngredientData => ingredientData;
    public DishData DishData => dishData;
    public int Amount => amount;    
}

[CreateAssetMenu(fileName = "DS_", menuName = "GameData/Food")]
public class DishData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string dishName;
    [SerializeField] private string reciepeGrade;
    [SerializeField] private int cost;
    [SerializeField] private int donation;
    [SerializeField] private int subscribers;
    [SerializeField] private DishMaterial[] materials;
    [SerializeField] private bool isSpecial;
    [SerializeField] private string info;

    public string ID => id;
    public string DishName => dishName;
    public string ReciepeGrade => reciepeGrade;
    public int Cost => cost;
    public int Donation => donation;
    public int Subscribers => subscribers;
    public DishMaterial[] Materials => materials;
    public bool IsSpecial => isSpecial;
    public string Info => info;

    public void SetData(string id, string dishName, string reciepeGrade, int cost, int donation, int subscribers, DishMaterial[] materials, bool isSpecial, string info)
    {
        this.id = id;
        this.dishName = dishName;
        this.reciepeGrade = reciepeGrade;
        this.cost = cost;
        this.donation = donation;
        this.subscribers = subscribers;
        this.materials = materials;
        this.isSpecial = isSpecial;
        this.info = info;
    }
}
