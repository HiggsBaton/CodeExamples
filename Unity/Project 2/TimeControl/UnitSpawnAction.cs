using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnAction
{
    public float time;
    public float curDelay;

    /// <summary>
    /// Create a new unit spawner state
    /// </summary>
    /// <param name="time">playback time, float</param>
    /// <param name="curDelay">time to next spawn, float</param>
    public UnitSpawnAction(float time, float curDelay) {
        this.time = time;
        this.curDelay = curDelay;
    }
}
