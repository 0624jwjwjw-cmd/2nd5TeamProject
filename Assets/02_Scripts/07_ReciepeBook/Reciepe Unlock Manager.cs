using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReciepeUnlockManager : MonoBehaviour, ISaveable
{
    public static ReciepeUnlockManager Instance { get; private set; }

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
        UnlockRecipe("TD_01");
        UnlockRecipe("BD_01");
    }
    public bool IsUnlocked(string foodID)
    {
        return unlockedRecipeIDs.Contains(foodID);        
    }
    public void UnlockRecipe(string foodID)
    {
        if (!GameDataRepository.Instance.TryGetDish(foodID, out _)) return;
        unlockedRecipeIDs.Add(foodID);
        OnUnlockChanged?.Invoke();
    }
    public void Save(SaveData data)
    {
        data.unlockedRecipeIDs = unlockedRecipeIDs.ToList();
    }
    public void Load(SaveData data)
    {
        unlockedRecipeIDs = new HashSet<string>(data.unlockedRecipeIDs);
    }
}
