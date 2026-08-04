using System;
using UnityEngine;

public class LiveManager : MonoBehaviour
{
    [SerializeField] private float _liveDuration = 20f;

    private float _currentTime;
    private bool _isLive;

    public bool IsLive => _isLive;
    public float CurrentTime => _currentTime;

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

        Debug.Log("라이브 종료");

        OnLiveEnded?.Invoke();
    }

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