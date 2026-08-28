using UnityEngine;

[CreateAssetMenu(fileName = "IV_", menuName = "GameData/Upgrade/Inventory")]
public class InventoryUpgradeData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private int level;
    [SerializeField] private int price;
    [SerializeField] private int stack;

    public string ID => id;
    public int Level => level;
    public int Price => price;
    public int Stack => stack;
    public void SetData(string id, int level, int price, int stack)
    {
        this.id = id;
        this.level = level;
        this.price = price;
        this.stack = stack;
    }
}
