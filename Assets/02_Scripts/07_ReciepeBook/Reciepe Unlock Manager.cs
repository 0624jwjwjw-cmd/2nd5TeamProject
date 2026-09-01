using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReciepeUnlockManager : MonoBehaviour
{
    public static ReciepeUnlockManager Instance { get; private set; }

    [SerializeField] private List<DishData> dishDatas;

    [SerializeField] public HashSet<string> unlockedRecipeIDs = new HashSet<string>();

    public event Action OnUnlockChanged;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //TestAllUnlock();
    }
    public bool IsUnlocked(string foodID)
    {
        return unlockedRecipeIDs.Contains(foodID);        
    }
    public void UnlockRecipe(string foodID)
    {
        foreach (DishData dishData in dishDatas)
        {
            if (dishData.ID == foodID)
            {
                unlockedRecipeIDs.Add(foodID);
            }
        }
        OnUnlockChanged?.Invoke();
    }
    public void TestAllUnlock()
    {
        if (unlockedRecipeIDs.Count >= dishDatas.Count)
        {
            return;
        }

        for (int i=0; i<dishDatas.Count; i++)
        {
            unlockedRecipeIDs.Add(dishDatas[i].ID);
        }
    }
}
