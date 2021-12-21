using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    #region Fields

    float tempGlobalTime;
    bool paused = false;
    Text timeCounter;
    Button restart;
    Button pause;
    Button play;
    Scrollbar timeBar;

    GoToTimeState timeChangeEvent;
    PauseEvent pauseEvent;

    #endregion

    #region Methods

    void Awake()
    {
        timeCounter = GameObject.FindGameObjectWithTag("TimeCounter").GetComponent<Text>();
        restart = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Button>();
        pause = GameObject.FindGameObjectWithTag("Pause").GetComponent<Button>();
        play = GameObject.FindGameObjectWithTag("Play").GetComponent<Button>();
        timeBar = GameObject.FindGameObjectWithTag("ScrollBar").GetComponent<Scrollbar>();

        play.gameObject.SetActive(false);
        timeBar.gameObject.SetActive(false);

        restart.onClick.AddListener(OnRestart);
        pause.onClick.AddListener(OnPause);
        play.onClick.AddListener(OnPlay);

        // does OnTimeBarChangeValue EVERY FRAME :)
        // disable for lags
        // timeBar.onValueChanged.AddListener(OnTimeBarChangeValue);

        // Must have event trigger component on the scrollbar
        EventTrigger trigger = timeBar.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnTimeBarChangeValue(timeBar.value); });
        trigger.triggers.Add(entry);
    }

    void Start() {
        GlobalTime.Time = 0;
        timeChangeEvent = new GoToTimeState();
        pauseEvent = new PauseEvent();

        EventManager.AddTimeChangeInvoker(this);
        EventManager.AddPauseTimeInvoker(this);
    }

    void Update()
    {
        if (!paused)
        {
            GlobalTime.Time += Time.deltaTime;
            timeCounter.text = "Playback time: " + GlobalTime.Time.ToString("0.0");
        }
    }

    /// <summary>
    /// Restart the scene
    /// </summary>
    void OnRestart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Pauses the gameplay
    /// Shows pause menu
    /// </summary>
    void OnPause()
    {
        Pause();
    }

    /// <summary>
    /// Resumes the gameplay
    /// Hides pause menu
    /// </summary>
    void OnPlay() {
        timeBar.value = 1;
        if (tempGlobalTime > 0) GlobalTime.Time = tempGlobalTime;
        
        Pause();
    }

    /// <summary>
    /// Handles showing and hiding UI elements
    /// 
    /// Also re-creates the pause event for future use
    /// </summary>
    void Pause() {
        restart.gameObject.SetActive(paused);
        pause.gameObject.SetActive(paused);
        play.gameObject.SetActive(!paused);
        timeBar.gameObject.SetActive(!paused);

        paused = !paused;
        pauseEvent.Invoke();
        pauseEvent = new PauseEvent();
        EventManager.AddPauseTimeInvoker(this);
    }

    /// <summary>
    /// What happens when we change the value in the scrollbar
    /// </summary>
    /// <param name="value">float</param>
    void OnTimeBarChangeValue(float value) {
        if (!Input.GetMouseButton(0))
        {
            float timeStamp = GlobalTime.Time * value;
            timeStamp = float.Parse(timeStamp.ToString("0.0"));
            timeChangeEvent.Invoke(timeStamp);
            // print("timeStamp " + timeStamp);
            timeChangeEvent = new GoToTimeState();
            EventManager.AddTimeChangeInvoker(this);
            tempGlobalTime = timeStamp;
            timeCounter.text = "Playback time: " + timeStamp.ToString("0.0");
        }
    }

    /// <summary>
    /// Add the listener to time change event
    /// </summary>
    /// <param name=""></param>
    public void AddTimeChangeListener(UnityAction<float> listener) {
        timeChangeEvent.AddListener(listener);
    }

    /// <summary>
    /// Add the listener to pause / unpause the game
    /// </summary>
    /// <param name=""></param>
    public void AddPauseTimeListener(UnityAction listener)
    {
        pauseEvent.AddListener(listener);
    }

    #endregion
}
