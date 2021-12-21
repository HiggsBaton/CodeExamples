using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    #region Fields

    [SerializeField] int healthPoints;
    [SerializeField] float speed;

    [SerializeField] float rotSpeed;

    Transform unitFinishPosition;

    Rigidbody thisBody;
    Collider thisCollider;
    Animator thisAnimator;

    UnitReachedFinish reachedFinish;
    UnitDied deathEvent;

    bool enabled = false;
    bool paused = false;

    [SerializeField] int recordEveryNFrames = 3;
    int frame = 3;

    List<UnitAction> recordedActions = new List<UnitAction>();

    #endregion

    #region Methods

    /// <summary>
    /// Set the goal position and start
    /// Initialize unit
    /// </summary>
    /// <param name="unitFinishPosition">Transform</param>
    public void Initialize(Transform unitFinishPosition) {
        this.unitFinishPosition = unitFinishPosition;

        thisBody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
        thisAnimator = GetComponent<Animator>();

        InitializeEvents();

        enabled = true;
        RecordState();
    }

    /// <summary>
    /// Initialization for all event-related things
    /// </summary>
    void InitializeEvents() {
        reachedFinish = new UnitReachedFinish();
        deathEvent = new UnitDied();

        EventManager.AddTimeChangeListener(GoToTime);
        EventManager.AddPauseTimeListener(Pause);
        EventManager.AddReachedFinishInvoker(this);
        EventManager.AddUnitDeadInvoker(this);
    }

    void Update()
    {
        if (enabled)
        {
            if (!paused)
            {
                float distance = Vector3.Distance(transform.position, unitFinishPosition.position);
                transform.position = Vector3.Lerp(transform.position, unitFinishPosition.position, (Time.deltaTime * speed) / distance);
                RotateTowardsGoal();
            }
        }

        // if not paused, record data once every N frames
        if (!paused) {
            frame--;
            if (frame <= 0)
            {
                RecordState();
                frame = recordEveryNFrames;
            }
        }
    }

    void Pause() {
        paused = !paused;

        RecordState();

        StopBodyFromMoving();

        thisAnimator.speed = paused == true ? 0 : 1;
    }

    /// <summary>
    /// Rotation didn't stop when paused or moving the time
    /// so i did this
    /// </summary>
    void StopBodyFromMoving() {
        thisCollider.enabled = !paused;
        thisBody.velocity = Vector3.zero;
        thisBody.useGravity = !paused;
        thisBody.freezeRotation = !paused;
        thisBody.Sleep();
    }

    /// <summary>
    /// Set rotation Y to face the object we are going to (unitFinishPosition)
    /// </summary>
    void RotateTowardsGoal() {
        float differenceZ = unitFinishPosition.position.z - transform.position.z;
        float differenceX = unitFinishPosition.position.x - transform.position.x;

        float atan = Mathf.Atan2(differenceX, differenceZ) * Mathf.Rad2Deg;
        //Debug.Log(atan);

        transform.rotation = Quaternion.Euler(0, atan, 0);
    }

    /// <summary>
    /// Deals damage and checks if HP is less than 0
    /// </summary>
    /// <param name="damageAmount">integer</param>
    public void ReceiveDamage(int damageAmount) {
        healthPoints -= damageAmount;
        if (healthPoints <= 0)
        {
            deathEvent.Invoke(transform);
            EnableObject(false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // what happens when Unit reaches the castle / finish
        if (collider.tag == "Finish")  {
            reachedFinish.Invoke(transform);
            EnableObject(false);
        }
    }

    void OnBecameInvisible() {
        deathEvent.Invoke(transform);
        EnableObject(false);
    }

    /// <summary>
    /// Add listener to the event of reaching the castle / finish obj
    /// </summary>
    /// <param name="listener"></param>
    public void AddReachedFinishListener(UnityAction<Transform> listener) {
        reachedFinish.AddListener(listener);
    }

    /// <summary>
    /// Add listener to the event of death
    /// so that towers will know the unit is no more
    /// </summary>
    /// <param name="listener"></param>
    public void AddUnitDeadListener(UnityAction<Transform> listener) {
        deathEvent.AddListener(listener);
    }

    /// <summary>
    /// Like SetActive(), but script is responsive to Events
    /// </summary>
    void EnableObject(bool state) {
        enabled = state;
        transform.GetChild(0).gameObject.SetActive(state);
        //thisCollider.enabled = paused == state ? state : paused;
        thisCollider.enabled = state;

        if (state) {
            if (reachedFinish == null) reachedFinish = new UnitReachedFinish();
            if (deathEvent == null) deathEvent = new UnitDied();
        }

        if (!state) {
            deathEvent.Invoke(transform);
        }
    }

    /// <summary>
    /// Create a new UnitAction and add to list
    /// </summary>
    void RecordState() {
        float t = float.Parse(GlobalTime.Time.ToString("0.0"));
        recordedActions.Add(new UnitAction(t, enabled, transform.position, transform.rotation.eulerAngles, healthPoints));
        //print("record at " + t);
    }

    /// <summary>
    /// changes object parameters to a value, recorded at a time
    /// </summary>
    /// <param name="time">playback time, float</param>
    void GoToTime(float time)
    {
        //print(gameObject.name + " GoToTime " + time);

        UnitAction state = recordedActions.Find(x => x.time == time);

        //UnitAction state = recordedActions[Mathf.FloorToInt(recordedActions.Count * time)];

        if (state != null)
        {
            EnableObject(state.isActive);
            transform.position = state.position;
            transform.rotation = Quaternion.Euler(state.rotation);
            healthPoints = state.HP;
        }
        else {
            // if object doesnt have a recorded state
            // it means that it didnt exist yet
            EnableObject(false);
        }

        StopBodyFromMoving();
    }

    #endregion
}
