using System;
using System.Collections;
using UnityEngine;

public class HeartManager : MonoBehaviour, ISaveable
{
    public static HeartManager Instance;

    [Header("Heart Setting")]
    [SerializeField] private int maxHeart = 10;
    private float recoverTime = 300f;
    // 마지막 시간 저장용
    private long lastHeartRecoverTime;

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
    private void Start()
    {
        StartCoroutine(HeartRecoveryRoutine());
    }

    // 게임 실행 중 회복
    private IEnumerator HeartRecoveryRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);


            RecoverHeart();
        }
    }



    private void RecoverHeart()
    {
        if (CurrencyManager.Instance.Heart >= maxHeart)
            return;

        DateTime lastTime = new DateTime(lastHeartRecoverTime);

        TimeSpan elapsed = DateTime.UtcNow - lastTime;



        int recoverCount =
            Mathf.FloorToInt(
                (float)elapsed.TotalSeconds / recoverTime
            );



        if (recoverCount > 0)
        {
            CurrencyManager.Instance.AddHeart(recoverCount);


            lastHeartRecoverTime =
                DateTime.UtcNow.Ticks;


            Debug.Log(
                $"하트 {recoverCount}개 회복"
            );
        }
    }




    // 라이브 시작 시 호출
    public bool UseHeart()
    {
        bool result = CurrencyManager.Instance.SpendHeart(1);
        if (result)
        {
            //회복 시작 시간 기록
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;
            SaveLoadManager.Instance.SetDirty();
        }
        return result;
    }


    // 게임 종료 시 시간 체크
    private void OnApplicationQuit()
    {
        SaveLastTime();
    }
    // 모바일 백그라운드로 일시정지할때 시간 체크
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLastTime();
        }
    }
    // 게임 종료 / 백그라운드
    private void SaveLastTime()
    {
        if (CurrencyManager.Instance.Heart < maxHeart)
        {
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;
            SaveLoadManager.Instance.SetDirty();
        }
    }

    // 저장,불러오기
    public void Save(SaveData data)
    {
        data.lastHeartRecoverTime = lastHeartRecoverTime;
    }
    public void Load(SaveData data)
    {
        lastHeartRecoverTime = data.lastHeartRecoverTime;
        //처음 시작할때 0이라서 현재 시간 업데이트 시킴
        if (lastHeartRecoverTime == 0)
        {
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;
        }
        RecoverHeart();
    }
}