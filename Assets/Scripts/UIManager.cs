using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button startStopButton;
    public Button pauseResumeButton;
    public Button doneButton;
    public Button settingsBtn;
    public Button closeSettingsBtn;

    public TextMeshProUGUI timerTextMesh;
    public TextMeshProUGUI leftPomodoroTextMesh;

    public TMP_InputField pomodoroInput;
    public TMP_InputField shortBreakInput;
    public TMP_InputField longBreakInput;

    public Sprite stopSprite;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    public GameObject settingsPanel;

    private void Awake()
    {
        SubscribeEvents();
        pauseResumeButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        EventManager.Instance.TriggerUpdateTimer($"{GetPomodoroTime():00}:00");
    }

    private int GetPomodoroTime()
    {
        return PlayerPrefs.HasKey(Constants.PlayerPrefs.POMODORO_TIME) ?
            PlayerPrefs.GetInt(Constants.PlayerPrefs.POMODORO_TIME) : 0;
    }

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStopPomodoro += OnStopPomodoro;
        EventManager.Instance.OnStartPomodoro += OnStartPomodoro;
        EventManager.Instance.OnPausePomodoro += OnPausePomodoro;
        EventManager.Instance.OnResumePomodoro += OnResumePomodoro;
        EventManager.Instance.OnUpdateTimer += OnUpdateTimer;
        EventManager.Instance.OnStartBreak += OnStartBreak;
        EventManager.Instance.OnDonePomodoro += OnDonePomodoro;
        EventManager.Instance.OnUpdateLeftPomodoroStatus += OnUpdateLeftPomodoroStatus;

        SubscribeButtonsEvent();
        SubscribeInputsEvent();
    }

    private void SubscribeButtonsEvent()
    {
        startStopButton.onClick.AddListener(StartPomodoro);
        pauseResumeButton.onClick.AddListener(PausePomodoro);
        doneButton.onClick.AddListener(DonePomodoro);
        settingsBtn.onClick.AddListener(OnClickSettingsBtn);
        closeSettingsBtn.onClick.AddListener(OnClickCloseSettingsBtn);
    }

    private void OnClickCloseSettingsBtn()
    {
        settingsPanel.gameObject.SetActive(false);
        EventManager.Instance.TriggerUpdateTimer($"{GetPomodoroTime():00}:00");
    }

    private void OnClickSettingsBtn()
    {
        pomodoroInput.text = PlayerPrefs.HasKey(Constants.PlayerPrefs.POMODORO_TIME) ?
                PlayerPrefs.GetInt(Constants.PlayerPrefs.POMODORO_TIME).ToString() :
                Pomodoro.defaultPomodoroMinutes.ToString();

        shortBreakInput.text = PlayerPrefs.HasKey(Constants.PlayerPrefs.SHORT_BREAK_TIME) ?
            PlayerPrefs.GetInt(Constants.PlayerPrefs.SHORT_BREAK_TIME).ToString() :
            Pomodoro.defaultShortBreakMinutes.ToString();

        longBreakInput.text = PlayerPrefs.HasKey(Constants.PlayerPrefs.LONG_BREAK_TIME) ?
            PlayerPrefs.GetInt(Constants.PlayerPrefs.LONG_BREAK_TIME).ToString() :
            Pomodoro.defaultLongBreakMinutes.ToString();

        settingsPanel.gameObject.SetActive(true);
    }

    private void SubscribeInputsEvent()
    {
        pomodoroInput.onValueChanged.AddListener((value) =>
        {
            TriggerSetMinuteValue(Constants.PlayerPrefs.POMODORO_TIME, value);
        });

        shortBreakInput.onValueChanged.AddListener((value) =>
        {
            TriggerSetMinuteValue(Constants.PlayerPrefs.SHORT_BREAK_TIME, value);
        });

        longBreakInput.onValueChanged.AddListener((value) =>
        {
            TriggerSetMinuteValue(Constants.PlayerPrefs.LONG_BREAK_TIME, value);
        });
    }

    private void TriggerSetMinuteValue(string key, string value)
    {
        if (!int.TryParse(value, out int parsedValue)) return;

        if (parsedValue <= 0) return;

        EventManager.Instance.TriggerSettingsMinute(key, parsedValue);
    }

    private void OnUpdateLeftPomodoroStatus(string status)
    {
        leftPomodoroTextMesh.text = status;
    }

    private void OnDonePomodoro()
    {
        this.startStopButton.gameObject.SetActive(true);
        this.doneButton.gameObject.SetActive(false);
    }

    private void DonePomodoro()
    {
        EventManager.Instance.TriggerDonePomodoro();
    }

    private void OnStartBreak()
    {
        this.pauseResumeButton.gameObject.SetActive(false);
        this.startStopButton.gameObject.SetActive(false);
        this.doneButton.gameObject.SetActive(true);
    }

    private void OnUpdateTimer(string time)
    {
        timerTextMesh.text = time;
    }

    private void OnResumePomodoro()
    {
        pauseResumeButton.GetComponent<Image>().sprite = resumeSprite;
        pauseResumeButton.onClick.RemoveListener(ResumePomodoro);
        pauseResumeButton.onClick.AddListener(PausePomodoro);
    }

    private void OnPausePomodoro()
    {
        pauseResumeButton.GetComponent<Image>().sprite = playSprite;
        pauseResumeButton.onClick.RemoveListener(PausePomodoro);
        pauseResumeButton.onClick.AddListener(ResumePomodoro);
    }

    private void OnStopPomodoro()
    {
        startStopButton.GetComponent<Image>().sprite = playSprite;
        startStopButton.gameObject.transform.localPosition = new Vector3(0, -69f, 0);
        startStopButton.onClick.RemoveListener(StopPomodoro);
        startStopButton.onClick.AddListener(StartPomodoro);

        EventManager.Instance.TriggerUpdateTimer($"{GetPomodoroTime():00}:00");
        pauseResumeButton.gameObject.SetActive(false);
    }

    private void OnStartPomodoro()
    {
        startStopButton.GetComponent<Image>().sprite = stopSprite;
        startStopButton.gameObject.transform.localPosition = new Vector3(132, -69f, 0);
        startStopButton.onClick.RemoveListener(StartPomodoro);
        startStopButton.onClick.AddListener(StopPomodoro);

        pauseResumeButton.gameObject.SetActive(true);
    }

    public void StartPomodoro()
    {
        EventManager.Instance.TriggerStartPomodoro();
    }

    public void StopPomodoro()
    {
        EventManager.Instance.TriggerStopPomodoro();
    }

    public void PausePomodoro()
    {
        EventManager.Instance.TriggerPausePomodoro();
    }

    public void ResumePomodoro()
    {
        EventManager.Instance.TriggerResumePomodoro();
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
        EventManager.Instance.OnUpdateTimer -= OnUpdateTimer;
        EventManager.Instance.OnStartBreak -= OnStartBreak;
        EventManager.Instance.OnUpdateLeftPomodoroStatus -= OnUpdateLeftPomodoroStatus;
    }
}
