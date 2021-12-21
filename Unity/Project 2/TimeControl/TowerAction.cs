using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAction
{
    public float time;
    public Vector3 rotation;
    public float curDelay;

    /// <summary>
    /// Create a new tower state
    /// </summary>
    /// <param name="time">playback time, float</param>
    /// <param name="rotation">rotation of the tower (as EulerAngles), Vector3</param>
    /// <param name="curDelay">time left to next shot, float</param>
    public TowerAction(float time, Vector3 rotation, float curDelay) {
        this.time = time;
        this.rotation = rotation;
        this.curDelay = curDelay;
    }
}
