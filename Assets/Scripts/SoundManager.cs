using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource _audio;
    [SerializeField]
    AudioClip _bellClip;
    [SerializeField]
    AudioClip _tickingClip;
    [SerializeField]
    AudioClip _clickClip;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        EventManager.Instance.OnStartPomodoro += OnStartPomodoro;
        EventManager.Instance.OnStopPomodoro += OnStopPomodoro;
        EventManager.Instance.OnPausePomodoro += OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro += OnResumePomodoro;
        EventManager.Instance.OnPomodoroFinished += OnPomodoroFinished;
        EventManager.Instance.OnStopBreak += OnStopBreak;
        EventManager.Instance.OnDonePomodoro += OnDonePomodoro;
    }

    void OnDonePomodoro() => _audio.PlayOneShot(_clickClip);

    void OnStopBreak() => _audio.PlayOneShot(_bellClip);

    void OnPomodoroFinished() => _audio.PlayOneShot(_bellClip);

    void OnResumePomodoro() => _audio.PlayOneShot(_tickingClip);

    void OnPausePomodoro()
    {
        _audio.Stop();
        _audio.PlayOneShot(_clickClip);
    }

    void OnStopPomodoro()
    {
        _audio.Stop();
        _audio.PlayOneShot(_clickClip);
    }

    void OnStartPomodoro() => _audio.PlayOneShot(_tickingClip);

    void OnDestroy()
    {
        UnsubscribeEvents();
    }

    void UnsubscribeEvents()
    {
        EventManager.Instance.OnStartPomodoro -= OnStartPomodoro;
        EventManager.Instance.OnStopPomodoro -= OnStopPomodoro;
        EventManager.Instance.OnPausePomodoro -= OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro -= OnResumePomodoro;
        EventManager.Instance.OnPomodoroFinished -= OnPomodoroFinished;
        EventManager.Instance.OnStopBreak -= OnStopBreak;
        EventManager.Instance.OnDonePomodoro -= OnDonePomodoro;
    }
}
