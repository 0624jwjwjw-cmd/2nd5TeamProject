using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class CookSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text count;

    private CookSlotItem slot;
    private CookSlots slots;

    public void Initialize(CookSlots slots)
    {
        this.slots = slots;
    }

    public void SetSlot(CookSlotItem slot)
    {
        this.slot = slot;
        gameObject.SetActive(true);
        iconImage.sprite = slot.ingredient.Data.Icon;
    }
}
