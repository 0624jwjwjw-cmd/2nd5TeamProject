using UnityEngine;
//using UnityEngine.InputSystem.iOS;

[CreateAssetMenu(fileName = "KC_",menuName ="GameData/Upgrade/Kitchen")]
public class KitchenUpgradeData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private int level;
    [SerializeField] private int price;
    [SerializeField] private int specialFoodRate;

    public string ID => id;
    public int Level => level;
    public int Price => price;
    public float SpecialFoodRate => specialFoodRate;

    public void SetData(string id, int level, int price, int specialFoodRate)
    {
        this.id = id;
        this.level = level;
        this.price = price;
        this.specialFoodRate = specialFoodRate;
    }
}
