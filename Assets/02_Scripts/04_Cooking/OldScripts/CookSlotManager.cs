//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.InputSystem;
//public class CookSlotItem
//{
//    public IngredientBase ingredient;
//    public int count;
//    public CookSlotItem(IngredientBase ingredient, int count)
//    {
//        this.ingredient = ingredient;
//        this.count = count;
//    }
//}
//public class CookSlotManager : MonoBehaviour
//{
//    [SerializeField] private int maxSlot = 3;
//    [SerializeField] public CookSlotUI[] solidSlotUIs;
//    [SerializeField] public GameObject[] dashedSlotUIs;
//    [SerializeField] public List<CookSlotItem> slots = new List<CookSlotItem>();

//    private void GetIngredient()
//    {
//        if(Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            Vector2 mousPos = Mouse.current.position.ReadValue();
//            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousPos);
//            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

//            if (hit.collider == null) return;

//            if (hit.collider.TryGetComponent<IngredientBase>(out IngredientBase ingredient))
//            {
//                AddIngredient(ingredient); 
//            }
            
//        }
//    }
//    private void AddIngredient(IngredientBase ingredient)
//    {
//        if (ingredient == null || ingredient.Data == null)
//        {
//            return;
//        }

//        foreach (CookSlotItem slot in slots)
//        {
//            if (slot.ingredient.Data.ID == ingredient.Data.ID)
//            {
//                slot.count++;

//                CookSlotUI sameSlot = FindSameIngredient(ingredient);
//                if (sameSlot != null)
//                {
//                    sameSlot.AddIngredient(ingredient);
//                }
//                return;
//            }
//        }

//        if (slots.Count >= maxSlot)
//        {
//            return;
//        }

//        slots.Add(new CookSlotItem(ingredient, 1));

//        CookSlotUI emptySlot = FindEmptySlot();
//        if (emptySlot == null)
//        {
//            return;
//        }

//        emptySlot.gameObject.SetActive(true);
//        emptySlot.SetIngredient(ingredient);

//        int index = System.Array.IndexOf(solidSlotUIs, emptySlot);
//        dashedSlotUIs[index].SetActive(false);
//    }

//    public void ClearSlots()
//    {
//        slots.Clear();
//        for(int i=0; i<solidSlotUIs.Length;i++)
//        {
//            solidSlotUIs[i].gameObject.SetActive(false);
//            dashedSlotUIs[i].SetActive(true);
//        }
//    }
//    public CookSlotUI FindEmptySlot()
//    {
//        foreach(CookSlotUI slotUI in solidSlotUIs)
//        {
//            if(slotUI.gameObject.activeSelf)
//            {
//                continue;
//            }
//            return slotUI;
//        }
//        return null;        
//    }
//    public CookSlotUI FindSameIngredient(IngredientBase ingredient)
//    {
//        foreach(CookSlotUI slotUI in solidSlotUIs)
//        {
//            if(slotUI.image.sprite == ingredient.spriteRenderer.sprite)
//            {
//                return slotUI;
//            }
//        }
//        return null;
//    }
//    public void CookComplete()
//    {
//        slots.Clear();
//    }
//}
