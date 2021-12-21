using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAction
{
    public float time;
    public bool isActive;
    public Vector3 position;

    /// <summary>
    /// Create a new bullet state
    /// </summary>
    /// <param name="time">playback time, float</param>
    /// <param name="isActive">is unit enabled?, bool</param>
    /// <param name="position">Vector3</param>
    public BulletAction(float time, bool isActive, Vector3 position) {
        this.time = time;
        this.isActive = isActive;
        this.position = position;
    }
}
