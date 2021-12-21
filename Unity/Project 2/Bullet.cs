using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    #region Fields

    [SerializeField] float speed;
    int damage;
    
    Transform target;
    Rigidbody thisBody;

    bool enabled = false;
    bool paused = false;

    [SerializeField] int recordEveryNFrames = 3;
    int frame = 3;

    List<BulletAction> recordedActions = new List<BulletAction>();

    #endregion

    #region Methods

    /// <summary>
    /// Set target and damage
    /// </summary>
    /// <param name="target">Transform</param>
    /// <param name="damage">integer</param>
    public void SetTarget(Transform target, int damage) {
        enabled = true;
        this.target = target;
        this.damage = damage;
        thisBody = GetComponent<Rigidbody>();
        EventManager.AddTimeChangeListener(GoToTime);
        EventManager.AddPauseTimeListener(Pause);
        RecordState();
    }

    void Update()
    {
        if (enabled)
        {
            if (!paused)
            {
                if (target != null)
                {
                    Vector3 direction = target.position - transform.position;
                    direction.Normalize();
                    transform.Translate(direction * speed * Time.deltaTime, Space.World);
                }
                else
                {
                    EnableObject(false);
                }
            }
        }

        // if not paused, record data once every N frames
        if (!paused)
        {
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
        paused = !paused;
        RecordState();

        StopBodyFromMoving();
    }

    /// <summary>
    /// Rotation didn't stop when paused or moving the time
    /// so i did this
    /// 
    /// it is less noticeable in bullets, but they also move
    /// </summary>
    void StopBodyFromMoving()
    {
        thisBody.velocity = Vector3.zero;
        thisBody.useGravity = !paused;
        thisBody.freezeRotation = !paused;
        thisBody.Sleep();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Unit") {
            collision.gameObject.GetComponent<Unit>().ReceiveDamage(damage);
        }

        // if we collide with anything, disable the bullet object
        EnableObject(false);
    }

    /// <summary>
    /// Create a new UnitAction and add to list
    /// </summary>
    void RecordState()
    {
        float t = float.Parse(GlobalTime.Time.ToString("0.0"));
        recordedActions.Add(new BulletAction(t, enabled, transform.position));
        //print("record at " + t);
    }

    /// <summary>
    /// changes object parameters to a value, recorded at a time
    /// </summary>
    /// <param name="time">playback time, float</param>
    void GoToTime(float time) {
        BulletAction state = recordedActions.Find(x => x.time == time);

        if (state != null)
        {
            EnableObject(state.isActive);
            transform.position = state.position;
        }
        else
        {
            // if object doesnt have a recorded state
            // it means that it didnt exist yet
            EnableObject(false);
        }

        StopBodyFromMoving();
    }

    /// <summary>
    /// Like SetActive(), but script is responsive to Events
    /// </summary>
    void EnableObject(bool state)
    {
        enabled = state;
        transform.GetChild(0).gameObject.SetActive(state);
        GetComponent<Collider>().enabled = state;
    }

    #endregion
}
