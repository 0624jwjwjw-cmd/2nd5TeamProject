using UnityEngine;
using System;
public class CurrencyManager : MonoBehaviour, ISaveable
{
    public static CurrencyManager Instance { get; private set; }


    [Header("현재 보유 재화")]
    [SerializeField] private int gold;
    [SerializeField] private int subscriber;
    [SerializeField] private int heart;


    public int Gold => gold;
    public int Subscriber => subscriber;
    public int Heart => heart;

    public event Action OnRevenueChanged;//후원금이랑 구독자 변경될때마다 보고용
    public event Action OnHeartChanged;//하트 변경될때마다 보고용
  
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // 정산 스크립트가 재화 계산하고 currencyManager에게 전달하는 통로
    public void AddGold(int amount)
    {
        gold += amount;
        OnRevenueChanged?.Invoke();
        SaveLoadManager.Instance.SetDirty();
    }
    public void AddSubscriber(int amount)
    {
        subscriber += amount;
        OnRevenueChanged?.Invoke();
        SaveLoadManager.Instance.SetDirty();
    }
    public void AddHeart(int amount)
    {
        heart += amount;
        OnHeartChanged?.Invoke();
        SaveLoadManager.Instance.SetDirty();
    }



    // 재화 사용
    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;
        gold -= amount;
        OnRevenueChanged?.Invoke();
        SaveLoadManager.Instance.SetDirty();
        return true;
    }
    public bool SpendHeart(int amount)
    {
        if (heart < amount)
            return false;

        heart -= amount;
        OnHeartChanged?.Invoke();
        SaveLoadManager.Instance.SetDirty();

        return true;
    }


    // 저장,불러오기
    public void Save(SaveData data)
    {
        data.gold = gold;
        data.subscriber = subscriber;
        data.heart = heart;
    }
    public void Load(SaveData data)
    {
        gold = data.gold;
        subscriber = data.subscriber;
        heart = data.heart;

        OnRevenueChanged?.Invoke();
        OnHeartChanged?.Invoke();
    }
}