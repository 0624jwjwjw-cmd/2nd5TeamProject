using UnityEngine;
using System;
public class GameDateManager : MonoBehaviour, ISaveable
{
    public static GameDateManager Instance;
    public event Action<int> OnDateChanged;

    [Header("Date Setting")]
    [SerializeField] private int maxDateCount = 5;

    private int dateCount = 0;

    // 하루에 필요한 총 횟수
    public int MaxDateCount => maxDateCount;
    public int DateCount => dateCount;

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
    public void AddDateCount()
    {
        dateCount++;
        SaveLoadManager.Instance.SetDirty();
        OnDateChanged?.Invoke(dateCount);
    }

    // 저장
    public void Save(SaveData data)
    {
        data.dateCount = dateCount;
    }

    // 불러오기
    public void Load(SaveData data)
    {
        dateCount = data.dateCount;
    }
}