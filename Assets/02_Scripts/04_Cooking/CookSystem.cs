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
    [SerializeField] private DishBase trash;
    [SerializeField] private CookSlots cookSlots;

    [SerializeField] private int specialRate;

    [SerializeField] private Transform cookResult;

    public void StartCook()
    {
        DishBase resultDish = FindMatchingDish(dishes);

        if (resultDish == null)
        {
            Instantiate(trash, cookResult.transform.position, Quaternion.identity);
            cookSlots.ClearSlots();
        }
        else
        {
            if(Random.Range(0,100) < specialRate)
            {
                int index = System.Array.IndexOf(dishes, resultDish);
                DishBase specialDish = specialDishes[index];
                Instantiate(specialDish, cookResult.transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(resultDish, cookResult.transform.position, Quaternion.identity);
            }
        }
        cookSlots.ClearSlots();
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
        if(dish.Materials.Length != cookSlots.slots.Count)
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
        foreach(CookSlotItem slot in cookSlots.slots)
        {
            if(slot.ingredient.ID == ingredientID)
            {
                return slot;
            }
        }
        return null;
    }
}
