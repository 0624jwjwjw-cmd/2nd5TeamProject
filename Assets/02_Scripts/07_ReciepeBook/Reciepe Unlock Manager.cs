using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReciepeUnlockManager : MonoBehaviour
{
    [SerializeField] private List<DishBase> dishBases;

    private HashSet<string> unlockedRecipeIDs = new HashSet<string>();
    
    public bool IsUnlocked(string foodID)
    {
        return unlockedRecipeIDs.Contains(foodID);        
    }
    public void UnlockRecipe(string recipeID)
    {
        foreach (DishBase dishbase in dishBases)
        {
            if (dishbase.ID == recipeID)
            {
                dishbase.Unlock();
            }
        }
        unlockedRecipeIDs.Add(recipeID);
    }
    public void TestUnlockRandomOne() //
    {
        dishBases[Random.Range(0, dishBases.Count)].Unlock();
    }
    public void TestAllUnlock()
    {
        for(int i=0; i>dishBases.Count; i++)
        {
            dishBases[i].Unlock();
        }
    }
}
