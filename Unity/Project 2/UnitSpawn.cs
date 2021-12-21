using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitSpawn : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject unitPrefab;
    [SerializeField] float spawnDelay;
    float curDelay;

    Transform unitStartPosition;
    Transform unitFinishPosition;

    [SerializeField] LayerMask pathwayLayer;

    List<UnitSpawnAction> recordedActions = new List<UnitSpawnAction>();

    bool paused = false;

    [SerializeField] int recordEveryNFrames = 3;
    int frame = 3;

    #endregion

    #region Methods

    void Start() {
        InitializeFields();
        InitializeTimer();
        EventManager.AddPauseTimeListener(Pause);
        EventManager.AddTimeChangeListener(GoToTime);
    }

    void Update()
    {
        if (!paused)
        {
            curDelay -= Time.deltaTime;
            if (curDelay <= 0)
            {
                // create a goblin at start position
                InstatiateUnit(unitStartPosition.position, unitStartPosition.rotation);
                InitializeTimer();
            }

            // let user spawn a unit with a click/touch
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, pathwayLayer))
                {
                    //Debug.DrawLine(ray.origin, hit.point, Color.green, 1f);
                    InstatiateUnit(hit.point, Quaternion.identity);
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

    void Pause() {
        RecordState();
        paused = !paused;
    }

    /// <summary>
    /// Fills all data required for the component to work
    /// </summary>
    void InitializeFields() {
        unitStartPosition = GameObject.FindGameObjectWithTag("Start").GetComponent<Transform>();
        unitFinishPosition = GameObject.FindGameObjectWithTag("Finish").GetComponent<Transform>();
    }

    /// <summary>
    /// Restart the timer
    /// </summary>
    void InitializeTimer()
    {
        curDelay = spawnDelay;        
    }

    /// <summary>
    /// Spawn our unit with specified position & rotation
    /// </summary>
    void InstatiateUnit(Vector3 position, Quaternion rotation) {
        GameObject newUnit = Instantiate(unitPrefab, position, rotation);
        // initialize the unit here
        // 
        newUnit.GetComponent<Unit>().Initialize(unitFinishPosition);
    }

    /// <summary>
    /// Create a new UnitAction and add to list
    /// </summary>
    void RecordState()
    {
        float t = float.Parse(GlobalTime.Time.ToString("0.0"));
        recordedActions.Add(new UnitSpawnAction(t, curDelay));
        //print("record at " + t);
    }

    /// <summary>
    /// changes object parameters to a value, recorded at a time
    /// </summary>
    /// <param name="time">playback time, float</param>
    void GoToTime(float time)
    {
        UnitSpawnAction state = recordedActions.Find(x => x.time == time);

        if (state != null)
        {
            curDelay = state.curDelay;
        }
        else
        {
            // leave as is
            //
            // unit spawner is present at the start of the game and will never be disabled
            // if we need to disable the unit spawner later, must add EnableObject() to the script
        }
    }

    #endregion
}
