using System;
using UnityEngine;

public class Pomodoro : MonoBehaviour
{
    public static byte defaultPomodoroMinutes = 25;
    public static byte defaultShortBreakMinutes = 5;
    public static byte defaultLongBreakMinutes = 15;
    float _pomodoroSeconds;
    float _shortBreakSeconds;
    float _longBreakSeconds;
    bool _pause;
    float _timer;
    float _secondTimer = 1f;
    bool _doingPomodoro;
    bool _doingBreak;
    byte _counter = 0;

    void Awake()
    {
        SubscribeEvents();
    }

    void Start()
    {
        InitSeconds();
        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(GetLeftPomodoroStatus());
    }

    void InitSeconds()
    {
        _pomodoroSeconds = GetSecondsByKey(Constants.PlayerPrefs.POMODORO_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.POMODORO_TIME);

        _shortBreakSeconds = GetSecondsByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME);

        _longBreakSeconds = GetSecondsByKey(Constants.PlayerPrefs.LONG_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.LONG_BREAK_TIME);
    }

    float? GetSecondsByKey(string key) =>
        PlayerPrefs.HasKey(key) ? (float)(PlayerPrefs.GetInt(key) * 60) : (float?)null;

    float GetDefaultSecondByKey(string key)
    {
        switch (key)
        {
            case Constants.PlayerPrefs.POMODORO_TIME:
                return (float)(defaultPomodoroMinutes * 60);
            case Constants.PlayerPrefs.SHORT_BREAK_TIME:
                return (float)(defaultShortBreakMinutes * 60);
            case Constants.PlayerPrefs.LONG_BREAK_TIME:
                return (float)(defaultLongBreakMinutes * 60);
            default:
                return 0f;
        }
    }

    void SetDefaultSeconds(string key)
    {
        switch (key)
        {
            case Constants.PlayerPrefs.POMODORO_TIME:
                PlayerPrefs.SetInt(key, defaultPomodoroMinutes * 60);
                break;
            case Constants.PlayerPrefs.SHORT_BREAK_TIME:
                PlayerPrefs.SetInt(key, defaultShortBreakMinutes * 60);
                break;
            case Constants.PlayerPrefs.LONG_BREAK_TIME:
                PlayerPrefs.SetInt(key, defaultLongBreakMinutes * 60);
                break;
        }
    }

    void SubscribeEvents()
    {
        EventManager.Instance.OnStopPomodoro += OnStopPomodoro;
        EventManager.Instance.OnStartPomodoro += OnStartPomodoro;
        EventManager.Instance.OnPausePomodoro += OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro += OnResumePomodoro;
        EventManager.Instance.OnPomodoroFinished += OnPomodoroFinished;
        EventManager.Instance.OnStartBreak += OnStartBreak;
        EventManager.Instance.OnStopBreak += OnStopBreak;
        EventManager.Instance.OnDonePomodoro += OnDonePomodoro;
        EventManager.Instance.OnSettingsMinute += OnSettingsMinute;
    }

    void OnSettingsMinute(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        InitSeconds();
    }

    void OnDonePomodoro()
    {
        _counter++;

        if (_counter > 4) _counter = 0;

        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(GetLeftPomodoroStatus());
        EventManager.Instance.TriggerStopPomodoro();
    }

    string GetLeftPomodoroStatus()
    {
        var leftPomodoro = 4 - _counter;

        return leftPomodoro > 0 ?
            $"{leftPomodoro} pomodoro left until long break!" :
            "Long pomodoro coming up next!";
    }

    void OnStopBreak() => _doingBreak = false;

    void OnStartBreak() =>
        _timer = _counter == 4 ? _longBreakSeconds : _shortBreakSeconds;

    void Update()
    {
        if (AllowToDoPomodoroTimer())
            DoPomodoroTimer(ref _timer, ref _secondTimer, Time.deltaTime);

        if (AllowToDoBreakTimer())
            DoBreakTimer(ref _timer, ref _secondTimer, Time.deltaTime);
    }

    void DoPomodoroTimer(ref float timer, ref float secondTimer, float deltaTime)
    {
        if (TimeIsOver(timer))
        {
            EventManager.Instance.TriggerPomodoroFinished();
            return;
        }

        timer -= deltaTime;
        secondTimer -= deltaTime;

        HandleDisplayTimer(secondTimer);
    }

    bool TimeIsOver(float timer) => timer <= 0f;

    bool AllowToDoPomodoroTimer()
    {
        if (_pause || !_doingPomodoro) return false;

        return true;
    }

    void DoBreakTimer(ref float timer, ref float secondTimer, float deltaTime)
    {
        if (TimeIsOver(timer))
        {
            EventManager.Instance.TriggerStopBreak();
            return;
        }

        timer -= deltaTime;
        secondTimer -= deltaTime;

        HandleDisplayTimer(secondTimer);
    }

    bool AllowToDoBreakTimer() => _doingBreak;

    void OnResumePomodoro() => _pause = false;

    void OnPausePomodoro() => _pause = true;

    void OnStartPomodoro()
    {
        _doingPomodoro = true;
        _timer = _pomodoroSeconds;
        _pause = false;
    }

    void OnStopPomodoro()
    {
        _timer = _pomodoroSeconds;
        _doingPomodoro = false;
        _doingBreak = false;
    }

    void OnPomodoroFinished()
    {
        _doingPomodoro = false;
        _doingBreak = true;

        EventManager.Instance.TriggerStartBreak();
    }

    void HandleDisplayTimer(float secondTimer)
    {
        if (_secondTimer <= 0)
        {
            ResetSecondTimer();
            DisplayTimer(_timer);
        }
    }

    void ResetSecondTimer() => this._secondTimer = 1f;

    void DisplayTimer(float timer)
    {
        var minutes = timer / 60f;
        var seconds = ((float)minutes - (int)minutes) * 60f;

        var timeStr = $"{(byte)minutes:00}:{(byte)seconds:00}";
        EventManager.Instance.TriggerUpdateTimer(timeStr);
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
    }

    void UnsubscribeEvents()
    {
        EventManager.Instance.OnStopPomodoro -= OnStopPomodoro;
        EventManager.Instance.OnStartPomodoro -= OnStartPomodoro;
        EventManager.Instance.OnPausePomodoro -= OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro -= OnResumePomodoro;
        EventManager.Instance.OnPomodoroFinished -= OnPomodoroFinished;
        EventManager.Instance.OnStartBreak -= OnStartBreak;
        EventManager.Instance.OnStopBreak -= OnStopBreak;
        EventManager.Instance.OnDonePomodoro -= OnDonePomodoro;
        EventManager.Instance.OnSettingsMinute -= OnSettingsMinute;
    }
}
