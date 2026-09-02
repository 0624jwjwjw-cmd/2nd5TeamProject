using System;
using Unity.VisualScripting;
using UnityEngine;

public class StudioUpgradeManager : MonoBehaviour, ISaveable
{
    public static StudioUpgradeManager Instance { get; private set; }

    [SerializeField] private StudioUpgradeData[] studioUpgradeDatas;

    [SerializeField] private StudioUpgradeData currentData;
    [SerializeField] private StudioUpgradeData nextData;

    public StudioUpgradeData CurrentData => currentData;
    public StudioUpgradeData NextData => nextData;

    [SerializeField] private int index = 0;

    public event Action OnStudioUpgradeChanged;
    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        if (index<studioUpgradeDatas.Length-1)
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
        currentData = studioUpgradeDatas[index];
        if(index + 1 < studioUpgradeDatas.Length)
        {
            nextData = studioUpgradeDatas[index + 1];
        }
        else
        {
            nextData = null;
        }
        OnStudioUpgradeChanged?.Invoke();
    }
    public void Save(SaveData data)
    {
        data.studioLevel = index;
    }
    public void Load(SaveData data)
    {
        index = data.studioLevel;
    }

}
