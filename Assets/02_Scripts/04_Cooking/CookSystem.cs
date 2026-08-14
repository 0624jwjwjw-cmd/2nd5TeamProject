using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
using TMPro.EditorUtilities;

public class CookSystem : MonoBehaviour
{
    [SerializeField] private DishBase[] dishes;
    [SerializeField] private DishBase[] specialDishes;
    [SerializeField] private DishBase trashfood;
    [SerializeField] private DishBase burnedFood;
    [SerializeField] private CookSlotManager cookSlotManager;

    [SerializeField] private int specialRate;

    [SerializeField] private Transform cookResult;

    public List<GameObject> madeDish = new List<GameObject>();

    public void StartCook()
    {
        DishBase resultDish = FindMatchingDish(dishes);

        if (resultDish == null)
        {
            DishBase trash = Instantiate(trashfood, cookResult.transform.position, Quaternion.identity);
            madeDish.Add(trash.gameObject);
        }
        else
        {
            if(!ReciepeUnlockManager.Instance.IsUnlocked(resultDish.ID))
            {
                DishBase burnedDish = Instantiate(burnedFood, cookResult.transform.position, Quaternion.identity);
                madeDish.Add(burnedDish.gameObject);
            }
            else
            {
                if (Random.Range(0, 100) < specialRate)
                {
                    int index = System.Array.IndexOf(dishes, resultDish);
                    DishBase specialDish = specialDishes[index];
                    DishBase madeSpecialDish = Instantiate(specialDish, cookResult.transform.position, Quaternion.identity);
                    madeDish.Add(madeSpecialDish.gameObject);
                }
                else
                {
                    DishBase dish = Instantiate(resultDish, cookResult.transform.position, Quaternion.identity);
                    madeDish.Add(dish.gameObject);
                }
            }
        }

        foreach (CookSlotItem slot in cookSlotManager.slots)
        {
            InventoryManager.Instance.RemoveItem(slot.ingredient.Data.ID, slot.count);
        }

        cookSlotManager.ClearSlots();
        for(int i=0;i<cookSlotManager.solidSlotUIs.Length;i++)
        {
            cookSlotManager.solidSlotUIs[i].Clear();
        }
    }
    private DishBase FindMatchingDish(DishBase[] dishBases)
    {
        for (int i = 0; i< dishBases.Length; i++)
        {
            DishData dish = dishBases[i].Data;

            if(CanCook(dish))
            {
                return dishBases[i];
            }
        }
        return null;
    }
    private bool CanCook(DishData dish)
    {
        if(dish.Materials.Length != cookSlotManager.slots.Count)
        {
            return false;
        }

        for(int i= 0; i< dish.Materials.Length; i++)
        {
            IngredientData material = dish.Materials[i].IngredientData;
            int requiredAmount = dish.Materials[i].Amount;

            CookSlotItem matchedSlot = FindSlotByID(material.ID);

            if(matchedSlot == null || matchedSlot.count != requiredAmount)
            {
                return false;
            }            
        }
        return true;
    }
    private CookSlotItem FindSlotByID(string ingredientID)
    {
        foreach(CookSlotItem slot in cookSlotManager.slots)
        {
            if(slot.ingredient.ID == ingredientID)
            {
                return slot;
            }
        }
        return null;
    }
    public void ClearDish()
    {
        foreach (GameObject dish in madeDish)
        {
            if (dish != null)
            {
                Destroy(dish);
            }
        }
        madeDish.Clear();
    }
}
