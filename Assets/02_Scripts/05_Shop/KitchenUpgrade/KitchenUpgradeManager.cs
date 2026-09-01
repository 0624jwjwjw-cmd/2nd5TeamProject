using UnityEngine;
using System;
public class KitchenUpgradeManager : MonoBehaviour
{
    public static KitchenUpgradeManager Instance { get; private set; }

    [SerializeField] private KitchenUpgradeData[] kitchenUpgradeDatas;

    [SerializeField] private KitchenUpgradeData currentData;
    [SerializeField] private KitchenUpgradeData nextData;

    public KitchenUpgradeData CurrentData => currentData;
    public KitchenUpgradeData NextData => nextData;

    [SerializeField] private int index = 0;

    public event Action OnKitchenUpgradeChanged;
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
        if (index < kitchenUpgradeDatas.Length - 1)
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
        currentData = kitchenUpgradeDatas[index];
        if (index + 1 < kitchenUpgradeDatas.Length)
        {
            nextData = kitchenUpgradeDatas[index + 1];
        }
        else
        {
            nextData = null;
        }
        OnKitchenUpgradeChanged?.Invoke();
    }
}
