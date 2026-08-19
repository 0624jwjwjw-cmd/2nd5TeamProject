using UnityEngine;

public class FoodPlace : MonoBehaviour
{
    public bool IsFilled => !string.IsNullOrEmpty(ItemId);

    public string ItemId { get; private set; }

    public bool TryPlace(string itemId)
    {
        if (IsFilled)
            return false;

        if (string.IsNullOrEmpty(itemId))
            return false;

        ItemId = itemId;
        return true;
    }

    public void RemoveFood()
    {
        ItemId = null;
    }

    public void Clear()
    {
        RemoveFood();
    }
}