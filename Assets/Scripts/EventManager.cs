using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EventManager : MonoBehaviour
{
    private static readonly EventManager _instance = new EventManager();

    public static EventManager Instance
    {
        get => _instance;
    }

    public delegate void StopAction();
    public event StopAction OnStopPomodoro;

    public delegate void StartAction();
    public event StartAction OnStartPomodoro;

    public delegate void PauseAction();
    public event PauseAction OnPausePomodoro;

    public delegate void ResumeAction();
    public event ResumeAction OnResumePomodoro;

    public delegate void UpdateTimerAction(string time);
    public event UpdateTimerAction OnUpdateTimer;

    public delegate void PomodoroFinishedAction();
    public event PomodoroFinishedAction OnPomodoroFinished;

    public delegate void StartBreakAction();
    public event StartBreakAction OnStartBreak;

    public delegate void StopBreakAction();
    public event StopBreakAction OnStopBreak;

    public delegate void DonePomodoroAction();
    public event DonePomodoroAction OnDonePomodoro;

    public delegate void UpdateLeftPomodoroStatusAction(string status);
    public event UpdateLeftPomodoroStatusAction OnUpdateLeftPomodoroStatus;

    public delegate void SettingsMinuteAction(string key, int value);
    public event SettingsMinuteAction OnSettingsMinute;

    public void TriggerStopPomodoro() => OnStopPomodoro?.Invoke();

    public void TriggerStartPomodoro() => OnStartPomodoro?.Invoke();

    public void TriggerPausePomodoro() => OnPausePomodoro?.Invoke();

    public void TriggerResumePomodoro() => OnResumePomodoro?.Invoke();

    public void TriggerUpdateTimer(string time) => OnUpdateTimer?.Invoke(time);

    public void TriggerPomodoroFinished() => OnPomodoroFinished?.Invoke();

    public void TriggerStopBreak() => OnStopBreak?.Invoke();

    public void TriggerStartBreak() => OnStartBreak?.Invoke();

    public void TriggerDonePomodoro() => OnDonePomodoro?.Invoke();

    public void TriggerUpdateLeftPomodoroStatus(string status) =>
        OnUpdateLeftPomodoroStatus?.Invoke(status);

    public void TriggerSettingsMinute(string key, int value) => OnSettingsMinute?.Invoke(key, value);
}
