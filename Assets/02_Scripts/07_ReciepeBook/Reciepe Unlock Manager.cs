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
    public void UnlockRecipe(string foodID)
    {
        foreach (DishBase dishbase in dishBases)
        {
            if (dishbase.ID == foodID)
            {
                unlockedRecipeIDs.Add(foodID);
            }
        }
    }
    public void TestUnlockRandomOne()
    {
        unlockedRecipeIDs.Add(dishBases[Random.Range(0, dishBases.Count)].ID);
    }
    public void TestAllUnlock()
    {
        for(int i=0; i<dishBases.Count; i++)
        {
            unlockedRecipeIDs.Add(dishBases[i].ID);
        }
    }
}
