using System;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    [SerializeField] private float _liveDuration = 20f;

    private float _elapsedTime;
    private bool _isLive;

    // 마지막으로 UI에 전달한 초
    private int _lastSecond;

    public bool IsLive => _isLive;
    public float ElapsedTime => _elapsedTime;

    public event Action OnLiveStarted;
    public event Action OnLiveEnded;
    public event Action<float> OnLiveTimeChanged;

    private void Awake()
    {
        _elapsedTime = 0f;
        _lastSecond = 0;
    }

    private void Update()
    {
        if (!_isLive)
        {
            return;
        }

        UpdateLiveTimer();
    }

    public void StartLive()
    {
        if (_isLive)
        {
            return;
        }

        _isLive = true;
        _elapsedTime = 0f;
        _lastSecond = 0;

        Debug.Log("라이브 시작!");

        OnLiveStarted?.Invoke();
    }

    public void EndLive()
    {
        if (!_isLive)
        {
            return;
        }

        _isLive = false;

        Debug.Log("라이브 종료");

        OnLiveEnded?.Invoke();
    }

    // 1초마다 이벤트 발생
    private void UpdateLiveTimer()
    {
        _elapsedTime += Time.deltaTime;

        int currentSecond = Mathf.FloorToInt(_elapsedTime);

        if (currentSecond > _lastSecond)
        {
            _lastSecond = currentSecond;
            OnLiveTimeChanged?.Invoke(currentSecond);
        }

        if (_elapsedTime >= _liveDuration)
        {
            EndLive();
        }
    }
}