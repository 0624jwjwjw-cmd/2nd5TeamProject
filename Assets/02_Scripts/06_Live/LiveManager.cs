using System;
using TMPro;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    [SerializeField] private float _liveDuration = 20f;

    [SerializeField] private TMP_Text _liveStatusText;
    [SerializeField] private TMP_Text _liveTimerText;

    private float _currentTime;
    private bool _isLive;

    public bool IsLive => _isLive;
    public float CurrentTime => _currentTime;

    // 라이브 시작-종료, 시간 변경을 UI, 채팅 등에 알리는 이벤트
    public event Action OnLiveStarted;
    public event Action OnLiveEnded;
    public event Action<float> OnLiveTimeChanged;

    private void Awake()
    {
        _currentTime = _liveDuration;
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
        _currentTime = _liveDuration;

        _liveStatusText.text = "LIVE ON";

        Debug.Log("라이브 시작!");

        OnLiveStarted?.Invoke();
        OnLiveTimeChanged?.Invoke(_currentTime);
    }

    public void EndLive()
    {
        if (!_isLive)
        {
            return;
        }

        _isLive = false;

        _liveStatusText.text = "LIVE OFF";
        _liveTimerText.text = "00";

        Debug.Log("라이브 종료");

        OnLiveEnded?.Invoke();
    }

    // 타이머 감소와 종료 처리 한 곳에 관리
    private void UpdateLiveTimer()
    {
        _currentTime -= Time.deltaTime;

        OnLiveTimeChanged?.Invoke(_currentTime);

        if (_currentTime <= 0f)
        {
            EndLive();
        }
    }
}