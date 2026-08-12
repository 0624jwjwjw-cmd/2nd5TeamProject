using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReciepeUnlockManager : MonoBehaviour
{
    public static ReciepeUnlockManager Instance { get; private set; }

    [SerializeField] private List<DishBase> dishBases;

    [SerializeField] private HashSet<string> unlockedRecipeIDs = new HashSet<string>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
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
        if (unlockedRecipeIDs.Count >= dishBases.Count)
        {
            return;
        }

        string id = dishBases[Random.Range(0, dishBases.Count)].ID;
        if(unlockedRecipeIDs.Contains(id))
        {
            TestUnlockRandomOne();
        }
        unlockedRecipeIDs.Add(id);
    }
    public void TestAllUnlock()
    {
        if (unlockedRecipeIDs.Count >= dishBases.Count)
        {
            return;
        }

        for (int i=0; i<dishBases.Count; i++)
        {
            unlockedRecipeIDs.Add(dishBases[i].ID);
        }
    }
}
