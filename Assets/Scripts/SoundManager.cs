using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audio;

    [SerializeField]
    private AudioClip _bellClip;
    [SerializeField]
    private AudioClip _tickingClip;
    [SerializeField]
    private AudioClip _clickClip;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStartPomodoro += OnStartPomodoro;
        EventManager.Instance.OnStopPomodoro += OnStopPomodoro;
        EventManager.Instance.OnPausePomodoro += OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro += OnResumePomodoro;
        EventManager.Instance.OnPomodoroFinished += OnPomodoroFinished;
        EventManager.Instance.OnStopBreak += OnStopBreak;
        EventManager.Instance.OnDonePomodoro += OnDonePomodoro;
    }

    private void OnDonePomodoro() => _audio.PlayOneShot(_clickClip);

    private void OnStopBreak() => _audio.PlayOneShot(_bellClip);

    private void OnPomodoroFinished() => _audio.PlayOneShot(_bellClip);

    private void OnResumePomodoro() => _audio.PlayOneShot(_tickingClip);

    private void OnPausePomodoro()
    {
        _audio.Stop();
        _audio.PlayOneShot(_clickClip);
    }

    private void OnStopPomodoro()
    {
        _audio.Stop();
        _audio.PlayOneShot(_clickClip);
    }

    private void OnStartPomodoro() => _audio.PlayOneShot(_tickingClip);

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
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
