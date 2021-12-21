using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : MonoBehaviour
{
    #region Fields

    [SerializeField] int damage;
    [SerializeField] float shootDelay;
    float curDelay;
    [SerializeField] float attackRadius;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform barrel;

    [SerializeField] GameObject baseToRotate;

    AttackZone attackZone;

    List<Transform> targetsInRange = new List<Transform>();

    List<TowerAction> recordedActions = new List<TowerAction>();

    bool paused = false;

    [SerializeField] int recordEveryNFrames = 3;
    int frame = 3;

    #endregion

    #region Methods

    void Start()
    {
        attackZone = GetComponentInChildren<AttackZone>();
        attackZone.SetRadius(attackRadius);

        InitializeTimer();

        EventManager.AddPauseTimeListener(Pause);
        EventManager.AddTimeChangeListener(GoToTime);
        EventManager.AddReachedFinishListener(OnTargetLeft);
        EventManager.AddUnitDeadListener(OnTargetLeft);
    }

    void Update()
    {
        if (!paused)
        {
            curDelay -= Time.deltaTime;
            if (curDelay <= 0)
            {
                if (targetsInRange.Count > 0)
                {
                    RotateTowardsGoal(baseToRotate, targetsInRange[0]);
                    Shoot(targetsInRange[0]);
                    InitializeTimer();
                }
            }

            // if not paused, record data once every N frames
            frame--;
            if (frame <= 0)
            {
                RecordState();
                frame = recordEveryNFrames;
            }
        }
    }

    void Pause()
    {
        RecordState();
        paused = !paused;
    }

    /// <summary>
    /// Set rotation Y to face the object we are going to (unitFinishPosition)
    /// </summary>
    void RotateTowardsGoal(GameObject toRotate, Transform target)
    {
        float differenceZ = target.position.z - transform.position.z;
        float differenceX = target.position.x - transform.position.x;

        float atan = Mathf.Atan2(differenceX, differenceZ) * Mathf.Rad2Deg;
        //Debug.Log(atan);

        toRotate.transform.rotation = Quaternion.Euler(0, atan, 0);
    }

    // insert new target into list of targets in range
    public void OnTargetEntered(Transform target) {
        // check that its not already there
        if (!targetsInRange.Contains(target)) targetsInRange.Add(target);
    }

    // remove target from list of targets in range
    public void OnTargetLeft(Transform target) {
        targetsInRange.Remove(target);
    }

    /// <summary>
    /// Restart the timer
    /// </summary>
    void InitializeTimer() {
        curDelay = shootDelay;
    }

    /// <summary>
    /// Create a projectile and set its target
    /// </summary>
    void Shoot(Transform target) {
        GameObject bullet = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
        bullet.GetComponent<Bullet>().SetTarget(target, damage);
    }

    /// <summary>
    /// Create a new UnitAction and add to list
    /// </summary>
    void RecordState()
    {
        float t = float.Parse(GlobalTime.Time.ToString("0.0"));
        recordedActions.Add(new TowerAction(t, baseToRotate.transform.rotation.eulerAngles, curDelay));
        //print("record at " + t);
    }

    /// <summary>
    /// changes object parameters to a value, recorded at a time
    /// </summary>
    /// <param name="time">playback time, float</param>
    void GoToTime(float time)
    {
        TowerAction state = recordedActions.Find(x => x.time == time);

        if (state != null)
        {
            baseToRotate.transform.rotation = Quaternion.Euler(state.rotation);
            curDelay = state.curDelay;
        }
        else
        {
            // leave as is
            //
            // tower is present at the start of the game and will never be disabled
            // if we need to disable the tower later, must add EnableObject() to the script
        }
    }

    #endregion
}
