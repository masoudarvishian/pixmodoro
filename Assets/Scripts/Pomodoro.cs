using System;
using UnityEngine;

public class Pomodoro : MonoBehaviour
{
    public static byte defaultPomodoroMinutes = 25;
    public static byte defaultShortBreakMinutes = 5;
    public static byte defaultLongBreakMinutes = 15;

    private float _pomodoroSeconds;
    private float _shortBreakSeconds;
    private float _longBreakSeconds;

    private bool _pause;
    private float _timer;
    private float _secondTimer = 1f;
    private bool _doingPomodoro;
    private bool _doingBreak;
    private byte _counter = 0;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        InitSeconds();
        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(GetLeftPomodoroStatus());
    }

    private void InitSeconds()
    {
        _pomodoroSeconds = GetSecondsByKey(Constants.PlayerPrefs.POMODORO_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.POMODORO_TIME);

        _shortBreakSeconds = GetSecondsByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME);

        _longBreakSeconds = GetSecondsByKey(Constants.PlayerPrefs.LONG_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.LONG_BREAK_TIME);
    }

    private float? GetSecondsByKey(string key) =>
        PlayerPrefs.HasKey(key) ? (float)(PlayerPrefs.GetInt(key) * 60) : (float?)null;

    private float GetDefaultSecondByKey(string key)
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

    private void SetDefaultSeconds(string key)
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

    private void SubscribeEvents()
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

    private void OnSettingsMinute(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        InitSeconds();
    }

    private void OnDonePomodoro()
    {
        _counter++;

        if (_counter > 4) _counter = 0;

        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(GetLeftPomodoroStatus());
        EventManager.Instance.TriggerStopPomodoro();
    }

    private string GetLeftPomodoroStatus()
    {
        var leftPomodoro = 4 - _counter;

        return leftPomodoro > 0 ?
            $"{leftPomodoro} pomodoro left until long break!" :
            "Long pomodoro coming up next!";
    }

    private void OnStopBreak() => _doingBreak = false;

    private void OnStartBreak() =>
        _timer = _counter == 4 ? _longBreakSeconds : _shortBreakSeconds;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (AllowToDoPomodoroTimer())
            DoPomodoroTimer(ref _timer, ref _secondTimer, Time.deltaTime);

        if (AllowToDoBreakTimer())
            DoBreakTimer(ref _timer, ref _secondTimer, Time.deltaTime);
    }

    private void DoPomodoroTimer(ref float timer, ref float secondTimer, float deltaTime)
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

    private bool TimeIsOver(float timer) => timer <= 0f;

    private bool AllowToDoPomodoroTimer()
    {
        if (_pause || !_doingPomodoro) return false;

        return true;
    }

    private void DoBreakTimer(ref float timer, ref float secondTimer, float deltaTime)
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

    private bool AllowToDoBreakTimer() => _doingBreak;

    private void OnResumePomodoro() => _pause = false;

    private void OnPausePomodoro() => _pause = true;

    private void OnStartPomodoro()
    {
        _doingPomodoro = true;
        _timer = _pomodoroSeconds;
        _pause = false;
    }

    private void OnStopPomodoro()
    {
        _timer = _pomodoroSeconds;
        _doingPomodoro = false;
        _doingBreak = false;
    }

    private void OnPomodoroFinished()
    {
        _doingPomodoro = false;
        _doingBreak = true;

        EventManager.Instance.TriggerStartBreak();
    }

    private void HandleDisplayTimer(float secondTimer)
    {
        if (_secondTimer <= 0)
        {
            ResetSecondTimer();
            DisplayTimer(_timer);
        }
    }

    private void ResetSecondTimer() => this._secondTimer = 1f;

    private void DisplayTimer(float timer)
    {
        var minutes = timer / 60f;
        var seconds = ((float)minutes - (int)minutes) * 60f;

        var timeStr = $"{(byte)minutes:00}:{(byte)seconds:00}";
        EventManager.Instance.TriggerUpdateTimer(timeStr);
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
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
