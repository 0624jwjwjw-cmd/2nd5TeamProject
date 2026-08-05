using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class CookSlotItem
{
    public IngredientBase ingredient;
    public int count;
    public CookSlotItem(IngredientBase ingredient, int count)
    {
        this.ingredient = ingredient;
        this.count = count;
    }
}
public class CookSlots : MonoBehaviour
{
    [SerializeField] private int maxSlot = 3;

    [SerializeField] public List<CookSlotItem> slots = new List<CookSlotItem>();
    private void Update()
    {
        GetIngredient();
    }
    private void GetIngredient()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider == null) return;

            if (hit.collider.TryGetComponent<IngredientBase>(out IngredientBase ingredient))
            {
                AddIngredient(ingredient); 
            }
            //
        }
    }
    private void AddIngredient(IngredientBase ingredient)
    {
        if (ingredient == null || ingredient.Data == null)
        {
            return;
        }

        foreach(CookSlotItem slot in slots)
        {
            if(slot.ingredient.Data.ID == ingredient.Data.ID)
            {
                slot.count++;
                Debug.Log($"{ingredient.Data.ID} 수량 증가: {slot.count}");
                return;
            }
        }

        if(slots.Count >= maxSlot)
        {
            Debug.Log("더 추가 불가");
            return;
        }

        slots.Add(new CookSlotItem(ingredient, 1));
        Debug.Log($"{ingredient.ID} 추가완료");
        for(int i= 0; i<slots.Count; i++)
        {
            Debug.Log($"{i}번째 안에 있는 아이템 : {slots[i].ingredient.ID}, {slots[i].count}");
        }
        return;
    }
    public void ClearSlots()
    {
        slots.Clear();
    }
}
