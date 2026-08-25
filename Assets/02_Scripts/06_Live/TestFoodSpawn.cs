using UnityEngine;

public class TestFoodSpawn : MonoBehaviour
{
    private void Start()
    {
        for (int i = 1; i <= 20; i++)
        {
            string id = $"DS_{i:00}";
            InventoryManager.Instance.AddItem(id, 1, ItemType.Ingredient);
        }
    }
}