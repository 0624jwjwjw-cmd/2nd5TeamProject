using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    [Header("Saveable Objects")]
    [SerializeField] private MonoBehaviour[] saveableObjects;

    private List<ISaveable> saveables = new();

    private string savePath;
    private bool isDirty;
    private float timer;
    private float saveDelay = 10f;

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
            return;
        }

        savePath = Path.Combine(
            Application.persistentDataPath,
            "SaveData.json"
        );

        RegisterSaveables();

        LoadGame();
    }

    private void RegisterSaveables()
    {
        saveables.Clear();

        foreach (MonoBehaviour obj in saveableObjects)
        {
            if (obj == null)
                continue;

            if (obj is ISaveable saveable)
            {
                Register(saveable);
            }
            else
            {
                Debug.LogWarning(
                    $"{obj.name}은 ISaveable을 구현하지 않았습니다."
                );
            }
        }
    }

    private void Update()
    {
        if (!isDirty)
            return;

        timer += Time.deltaTime;

        if (timer >= saveDelay)
        {
            SaveGame();

            timer = 0f;
            isDirty = false;
        }
    }

    // 저장 필요 상태로 변경
    public void SetDirty()
    {
        isDirty = true;
        timer = 0f;
    }

    public void Register(ISaveable saveable)
    {
        if (saveable == null)
            return;

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

        string json = JsonUtility.ToJson(data, true);
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

        Debug.Log("저장 데이터 불러오기 완료");
    }

    // 앱 종료 대비
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // 앱 백그라운드 대비
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

    public void ResetGame()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        SaveData data = new SaveData();

        foreach (ISaveable saveable in saveables)
        {
            saveable.Load(data);
        }

        SaveGame();

        isDirty = false;
        timer = 0f;
    }
}