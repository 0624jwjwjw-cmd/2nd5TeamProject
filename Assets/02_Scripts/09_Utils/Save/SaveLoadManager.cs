using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveLoadManager : MonoBehaviour, IInitializable
{
    public static SaveLoadManager Instance;

    private List<ISaveable> saveables = new();
    private string savePath;
    private bool isDirty;
    private float timer;
    private float saveDelay = 10f;

    //게임 시작 순서 구현
    public int Priority => 50;
    public void Initialize()
    {
        var objects = FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None)
            .OfType<ISaveable>();

        foreach (ISaveable obj in objects)
        {
            Register(obj);
        }
        LoadGame();
    }

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
        savePath =Path.Combine(Application.persistentDataPath,"SaveData.json");
    }

    private void Update()
    {
        if (!isDirty)return;
        timer += Time.deltaTime;
        if (timer >= saveDelay)
        {
            SaveGame();
            timer = 0;
            isDirty = false;
        }
    }
    //저장 필요해지면 10초뒤에 변경
    public void SetDirty()
    {
        isDirty = true;
        timer = 0;
    }
    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
        {
            saveables.Add(saveable);
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        foreach (ISaveable saveable in saveables)
        {
            saveable.Save(data);
        }
        string json =JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("저장 데이터 없음");
            return;
        }
        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        foreach (ISaveable saveable in saveables)
        {
            saveable.Load(data);
        }
    }


    // 앱 종료 / 백그라운드 대비
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

    public void ResetGame()
    {
        // 저장 파일 삭제
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        // 기본 데이터 생성
        SaveData data = new SaveData();
        // 모든 매니저를 기본값으로 변경
        foreach (ISaveable saveable in saveables)
        {
            saveable.Load(data);
        }
        // 변경된 데이터를 즉시 저장
        SaveGame();

    }
}