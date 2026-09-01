using UnityEngine;
using System;
public class InventoryUpgradeManager : MonoBehaviour
{
    public static InventoryUpgradeManager Instance { get; private set; }

    [SerializeField] private InventoryUpgradeData[]inventoryUpgradeDatas;

    [SerializeField] private InventoryUpgradeData currentData;
    [SerializeField] private InventoryUpgradeData nextData;

    public InventoryUpgradeData CurrentData => currentData;
    public InventoryUpgradeData NextData => nextData;

    [SerializeField] private int index = 0;

    public event Action OnInventoryUpgradeChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        SetData(index);
    }
    public void LevelUp()
    {
        if(!CurrencyManager.Instance.SpendGold(nextData.Price))
        {
            return;
        }
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (index < inventoryUpgradeDatas.Length - 1)
        {
            index++;
            SetData(index);
        }
        else
        {
            return;
        }
    }
    private void SetData(int index)
    {
        currentData = inventoryUpgradeDatas[index];
        if (index + 1 < inventoryUpgradeDatas.Length)
        {
            nextData = inventoryUpgradeDatas[index + 1];
        }
        else
        {
            nextData = null;
        }
        InventoryManager.Instance.MaxStackSize = currentData.Stack;
        OnInventoryUpgradeChanged?.Invoke();
    }
}