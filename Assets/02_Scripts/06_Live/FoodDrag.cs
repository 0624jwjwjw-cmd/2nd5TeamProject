using UnityEngine;

public class FoodDrag : MonoBehaviour
{
    public static FoodDrag Instance { get; private set; }

    public string ItemId { get; private set; }
    public bool IsDragging { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void BeginDrag(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return;

        ItemId = itemId;
        IsDragging = true;
    }

    public void EndDrag()
    {
        ItemId = null;
        IsDragging = false;
    }

    public bool TryGetItemId(out string itemId)
    {
        itemId = ItemId;

        return IsDragging &&
               !string.IsNullOrWhiteSpace(ItemId);
    }
}