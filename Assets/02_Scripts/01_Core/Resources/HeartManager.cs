using System;
using System.Collections;
using UnityEngine;

public class HeartManager : MonoBehaviour, ISaveable
{
    public static HeartManager Instance;

    [Header("Heart Setting")]
    [SerializeField] private int maxHeart = 10;
    [SerializeField] private float recoverTime = 300f;
    //시간 저장용
    private long lastHeartRecoverTime;

    public void Start()
    {
        RecoverOfflineHeart();

        StartCoroutine(HeartRecoveryRoutine());
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
    }

    //초단위로 코루틴 가동
    private IEnumerator HeartRecoveryRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            RecoverHeartInGame();
        }
    }
    // 게임 중 하트 회복
    private void RecoverHeartInGame()
    {
        if (CurrencyManager.Instance.Heart >= maxHeart)
            return;

        TimeSpan elapsed = DateTime.UtcNow - new DateTime(lastHeartRecoverTime);//현재시간 - 마지막 회복 시간
        if (elapsed.TotalSeconds >= recoverTime)
        {
            CurrencyManager.Instance.AddHeart(1);
            
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;//하트 먹은 현재 시간 갱신
            Debug.Log("게임 중 하트 +1");
        }
    }
    private void RecoverOfflineHeart()
    {
        if (lastHeartRecoverTime == 0) return;//첫 실행 버그 방어용
        if (CurrencyManager.Instance.Heart >= maxHeart) return; //하트 최대치면 중단


        TimeSpan elapsed = DateTime.UtcNow - new DateTime(lastHeartRecoverTime);//현재시간 - 마지막 회복 시간을 변수로 잡기


        int recoverCount =Mathf.FloorToInt((float)elapsed.TotalSeconds / recoverTime);//경과시간 / 5분
        if (recoverCount <= 0) return;//0이면 중단
        int missingHeart = maxHeart - CurrencyManager.Instance.Heart;
        recoverCount = Mathf.Min(recoverCount, missingHeart);
        CurrencyManager.Instance.AddHeart(recoverCount);

        lastHeartRecoverTime +=TimeSpan.FromSeconds(recoverCount * recoverTime).Ticks;//하트 회복한 시간만큼 마지막 회복 시간 갱신

        Debug.Log($"오프라인 하트 {recoverCount}개 회복");
    }
    public bool UseHeart()
    {
        if (!CurrencyManager.Instance.SpendHeart(1)) return false;
        // 최대치에서 처음 감소했을 때만 시작 시간 기록
        if (CurrencyManager.Instance.Heart == maxHeart - 1)
        {
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;
        }
        SaveLoadManager.Instance.SetDirty();
        return true;
    }
    //5분 타이머 전달용
    public int GetRemainingRecoverTime()
    {
        // 하트 최대면 타이머 필요 없음
        if (CurrencyManager.Instance.Heart >= maxHeart)
            return 0;

        TimeSpan elapsed = DateTime.UtcNow - new DateTime(lastHeartRecoverTime);

        int remaining = Mathf.CeilToInt(recoverTime - (float)elapsed.TotalSeconds);

        return Mathf.Max(remaining, 0);
    }

    public void Save(SaveData data)
    {
        data.lastHeartRecoverTime = lastHeartRecoverTime;
    }
    public void Load(SaveData data)
    {
        lastHeartRecoverTime = data.lastHeartRecoverTime;

        // 첫 실행 이유 : long 데이터가 일반적으로 68400000000000으로 되어 있어서 이걸 하나하나 확인하면 터짐!!
        if (lastHeartRecoverTime == 0)
        {
            lastHeartRecoverTime = DateTime.UtcNow.Ticks;
        }
    }
}