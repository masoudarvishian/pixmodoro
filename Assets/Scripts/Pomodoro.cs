using UnityEngine;

public class Pomodoro : MonoBehaviour
{
    public const byte DefaultPomodoroMinutes = 25;
    public const byte DefaultShortBreakMinutes = 5;
    public const byte DefaultLongBreakMinutes = 15;
    private float _pomodoroSeconds;
    private float _shortBreakSeconds;
    private float _longBreakSeconds;
    private bool _pause;
    private float _timer;
    private float _secondTimer = 1f;
    private bool _doingPomodoro;
    private bool _doingBreak;
    private byte _counter;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        InitSeconds();
        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(
            GetLeftPomodoroStatus());
    }

    private void InitSeconds()
    {
        _pomodoroSeconds =
            GetSecondsByKey(Constants.PlayerPrefs.POMODORO_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.POMODORO_TIME);

        _shortBreakSeconds =
            GetSecondsByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.SHORT_BREAK_TIME);

        _longBreakSeconds =
            GetSecondsByKey(Constants.PlayerPrefs.LONG_BREAK_TIME) ??
            GetDefaultSecondByKey(Constants.PlayerPrefs.LONG_BREAK_TIME);
    }

    private static float? GetSecondsByKey(string key) =>
        PlayerPrefs.HasKey(key)
            ? PlayerPrefs.GetInt(key) * 60
            : (float?) null;

    private static float GetDefaultSecondByKey(string key)
    {
        switch (key)
        {
            case Constants.PlayerPrefs.POMODORO_TIME:
                return DefaultPomodoroMinutes * 60;
            case Constants.PlayerPrefs.SHORT_BREAK_TIME:
                return DefaultShortBreakMinutes * 60;
            case Constants.PlayerPrefs.LONG_BREAK_TIME:
                return DefaultLongBreakMinutes * 60;
            default:
                return 0f;
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

        EventManager.Instance.TriggerUpdateLeftPomodoroStatus(
            GetLeftPomodoroStatus());
        EventManager.Instance.TriggerStopPomodoro();
    }

    private string GetLeftPomodoroStatus()
    {
        var leftPomodoro = 4 - _counter;

        return leftPomodoro > 0
            ? $"{leftPomodoro} pomodoro left until long break!"
            : "Long pomodoro coming up next!";
    }

    private void OnStopBreak() => _doingBreak = false;

    private void OnStartBreak() =>
        _timer = _counter == 4 ? _longBreakSeconds : _shortBreakSeconds;

    private void Update()
    {
        if (AllowToDoPomodoroTimer())
            DoPomodoroTimer(ref _timer, ref _secondTimer, Time.deltaTime);

        if (AllowToDoBreakTimer())
            DoBreakTimer(ref _timer, ref _secondTimer, Time.deltaTime);
    }

    private void DoPomodoroTimer(ref float timer, ref float secondTimer,
        float deltaTime)
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

    private bool AllowToDoPomodoroTimer() => !_pause && _doingPomodoro;

    private void DoBreakTimer(ref float timer, ref float secondTimer,
        float deltaTime)
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
        if (!(secondTimer <= 0)) return;
        ResetSecondTimer();
        DisplayTimer(_timer);
    }

    private void ResetSecondTimer() => _secondTimer = 1f;

    private static void DisplayTimer(float timer)
    {
        var minutes = timer / 60f;
        var seconds = (minutes - (int) minutes) * 60f;

        var timeStr = $"{(byte) minutes:00}:{(byte) seconds:00}";
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