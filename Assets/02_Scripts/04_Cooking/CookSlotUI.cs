using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CookSlotUI : MonoBehaviour
{
    [SerializeField] private CookSlotManager cookSlotManager;

    public Image image;
    public TMP_Text count;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetIngredient(IngredientBase ingredient)
    {
        foreach(CookSlotItem cookSlotItem in cookSlotManager.slots)
        {
            if(cookSlotItem.ingredient.Data.ID == ingredient.ID)
            {
                gameObject.SetActive(true);
                image.sprite = ingredient.spriteRenderer.sprite;
                count.text = cookSlotItem.count.ToString();
            }
        }
    }
    public void AddIngredient(IngredientBase ingredient)
    {
        foreach(CookSlotItem cookSlotItem in cookSlotManager.slots)
        {
            if(cookSlotItem.ingredient.Data.ID == ingredient.ID)
            {
                int number = int.Parse(count.text);
                number++;
                count.text = number.ToString();
            }
        }
    }
    public void RemoveIngredient()
    {
        CookSlotItem targetSlot = null;

        foreach (CookSlotItem cookSlotItem in cookSlotManager.slots)
        {
            if (cookSlotItem.ingredient != null &&
                cookSlotItem.ingredient.spriteRenderer.sprite == image.sprite)
            {
                targetSlot = cookSlotItem;
                break;
            }
        }

        if (targetSlot == null) return;

        if (targetSlot.count > 1)
        {
            targetSlot.count--;
            count.text = targetSlot.count.ToString();
        }
        else
        {
            cookSlotManager.slots.Remove(targetSlot);
            Clear();
        }
    }
    public void Clear()
    {
        image.sprite = null;
        count.text = "";
        gameObject.SetActive(false);
    }
}
