using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenCookingSlot : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlotManager manager;

    public string ingredientID;
    public Image image;
    public TMP_Text count;
    public GameObject dashedSlot;

    public void SetIngredient(string ingredientID)
    {
        foreach (KitchenCookSlotItem cookSlotItem in manager.slots)
        {
            this.ingredientID = ingredientID;
            if (cookSlotItem.ingredientID == ingredientID)
            {
                gameObject.SetActive(true);
                if (ItemVisualRepository.Instance.TryGetIcon(ingredientID, out Sprite icon))
                {
                    image.sprite = icon;
                }
                else
                {
                    return;
                }
                count.text = cookSlotItem.count.ToString();
            }
        }
    }
    public void AddIngredient(string ingredientID)
    {
        foreach (KitchenCookSlotItem cookSlotItem in manager.slots)
        {
            if (cookSlotItem.ingredientID == ingredientID)
            {
                int number = int.Parse(count.text);
                number++;
                count.text = number.ToString();
            }
            this.ingredientID = ingredientID;
        }
    }
    public void RemoveIngredient()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        KitchenCookSlotItem targetSlot = null;

        foreach (KitchenCookSlotItem cookSlotItem in manager.slots)
        {
            if (cookSlotItem.ingredientID == this.ingredientID)
            {
                targetSlot = cookSlotItem;
                break;
            }
        }

        if (targetSlot == null)
        {
            return;
        }

        if (targetSlot.count > 1)
        {
            targetSlot.count--;
            count.text = targetSlot.count.ToString();
        }
        else
        {
            manager.slots.Remove(targetSlot);
            Clear();
        }
    }
    public void Clear()
    {
        image.sprite = null;
        count.text = "";
        gameObject.SetActive(false);
        dashedSlot.gameObject.SetActive(true);
    }
}
