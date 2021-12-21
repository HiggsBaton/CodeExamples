using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static List<Unit> reachedFinishInvokers = new List<Unit>();
    static List<UnityAction<Transform>> reachedFinishListeners = new List<UnityAction<Transform>>();

    static List<Unit> unitDeadInvokers = new List<Unit>();
    static List<UnityAction<Transform>> unitDeadListeners = new List<UnityAction<Transform>>();

    static UI timeChangeInvoker;
    static List<UnityAction<float>> timeChangeListeners = new List<UnityAction<float>>();

    static UI pauseTimeInvoker;
    static List<UnityAction> pauseTimeListeners = new List<UnityAction>();

    /// <summary>
    /// Add a new invoker (when new unit is spawned)
    /// </summary>
    /// <param name="invoker">Unit aka goblin</param>
    public static void AddReachedFinishInvoker(Unit invoker) {
        reachedFinishInvokers.Add(invoker);
        foreach (UnityAction<Transform> listener in reachedFinishListeners) {
            invoker.AddReachedFinishListener(listener);
        }
    }

    /// <summary>
    /// Add new listener (in case we make more than one tower)
    /// </summary>
    /// <param name="listener">CannonTower</param>
    public static void AddReachedFinishListener(UnityAction<Transform> listener) {
        reachedFinishListeners.Add(listener);
        foreach (Unit invoker in reachedFinishInvokers) {
            invoker.AddReachedFinishListener(listener);
        }
    }

    /// <summary>
    /// Add a new invoker (when new unit is spawned)
    /// </summary>
    /// <param name="invoker">Unit aka goblin</param>
    public static void AddUnitDeadInvoker(Unit invoker)
    {
        unitDeadInvokers.Add(invoker);
        foreach (UnityAction<Transform> listener in unitDeadListeners)
        {
            invoker.AddUnitDeadListener(listener);
        }
    }

    /// <summary>
    /// Add new listener (in case we make more than one tower)
    /// </summary>
    /// <param name="listener">CannonTower</param>
    public static void AddUnitDeadListener(UnityAction<Transform> listener)
    {
        unitDeadListeners.Add(listener);
        foreach (Unit invoker in unitDeadInvokers)
        {
            invoker.AddUnitDeadListener(listener);
        }
    }

    /// <summary>
    /// Add the invoker for time change
    /// (we need only one invoker)
    /// </summary>
    /// <param name="invoker">UI script</param>
    public static void AddTimeChangeInvoker(UI invoker)
    {
        timeChangeInvoker = invoker;
        foreach (UnityAction<float> listener in timeChangeListeners)
        {
            invoker.AddTimeChangeListener(listener);
        }
    }

    /// <summary>
    /// Add new listener for time control
    /// </summary>
    /// <param name="listener">Cannon, Unit, Bullet</param>
    public static void AddTimeChangeListener(UnityAction<float> listener)
    {
        timeChangeListeners.Add(listener);
        if (timeChangeInvoker != null) timeChangeInvoker.AddTimeChangeListener(listener);
    }

    /// <summary>
    /// Add the invoker for pause/unpause
    /// (we need only one invoker)
    /// </summary>
    /// <param name="invoker">UI script</param>
    public static void AddPauseTimeInvoker(UI invoker)
    {
        pauseTimeInvoker = invoker;
        foreach (UnityAction listener in pauseTimeListeners)
        {
            invoker.AddPauseTimeListener(listener);
        }
    }

    /// <summary>
    /// Add listener for pausing / unpausing
    /// </summary>
    /// <param name="listener">Cannon, Unit, Bullet</param>
    public static void AddPauseTimeListener(UnityAction listener)
    {
        pauseTimeListeners.Add(listener);
        if (pauseTimeInvoker != null) pauseTimeInvoker.AddPauseTimeListener(listener);
    }
}
