using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;


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
        }


        savePath =
        Path.Combine(Application.persistentDataPath,
        "SaveData.json");
    }



    private void Start()
    {
        var objects = FindObjectsOfType<MonoBehaviour>()
       .OfType<ISaveable>();


        foreach (ISaveable obj in objects)
        {
            Register(obj);
        }

        Debug.Log($"저장 가능한 개수 : {saveables.Count}");
        LoadGame();
    }



    private void Update()
    {
        if (!isDirty)
            return;


        timer += Time.deltaTime;


        if (timer >= saveDelay)
        {
            SaveGame();

            timer = 0;
            isDirty = false;
        }
    }



    public void SetDirty()
    {
        isDirty = true;

        // 새 변경이 생기면 타이머 초기화
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


        string json =
        JsonUtility.ToJson(data, true);


        File.WriteAllText(savePath, json);


        Debug.Log("전체 저장 완료");
    }




    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("저장 데이터 없음");
            return;
        }


        string json =
        File.ReadAllText(savePath);


        SaveData data =
        JsonUtility.FromJson<SaveData>(json);



        foreach (ISaveable saveable in saveables)
        {
            saveable.Load(data);
        }


        Debug.Log("전체 불러오기 완료");
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
}