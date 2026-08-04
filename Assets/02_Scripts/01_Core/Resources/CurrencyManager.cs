using UnityEngine;

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

        SaveLoadManager.Instance.SetDirty();
    }
    public void AddSubscriber(int amount)
    {
        subscriber += amount;

        SaveLoadManager.Instance.SetDirty();
    }
    public void AddHeart(int amount)
    {
        heart += amount;

        SaveLoadManager.Instance.SetDirty();
    }



    // 재화 사용
    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;
        gold -= amount;
        SaveLoadManager.Instance.SetDirty();
        return true;
    }
    public bool SpendHeart(int amount)
    {
        if (heart < amount)
            return false;

        heart -= amount;

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
        Debug.Log($"Load CurrencyManager: gold={gold}, subscriber={subscriber}, heart={heart}");
    }
}