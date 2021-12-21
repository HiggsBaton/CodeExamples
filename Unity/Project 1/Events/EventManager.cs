using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static List<ButtonScript> buttonPressInvoker = new List<ButtonScript>();
    static List<UnityAction<int>> buttonPressListener = new List<UnityAction<int>>();

    static List<BulletControl> freePrisonerInvoker = new List<BulletControl>();
    static List<UnityAction> freePrisonerListener = new List<UnityAction>();

    public static void AddButtonPressInvoker(ButtonScript invoker)
    {
        buttonPressInvoker.Add(invoker);
        foreach (UnityAction<int> listener in buttonPressListener)
        {
            invoker.AddButtonPressListener(listener);
        }
    }

    public static void AddButtonPressListener(UnityAction<int> listener)
    {
        buttonPressListener.Add(listener);
        foreach (ButtonScript myButton in buttonPressInvoker)
        {
            myButton.AddButtonPressListener(listener);
        }
    }

    public static void AddFreePrisonerInvoker(BulletControl invoker)
    {
        freePrisonerInvoker.Add(invoker);
        foreach (UnityAction listener in freePrisonerListener)
        {
            invoker.AddFreePrisonerListener(listener);
        }
    }

    public static void AddFreePrisonerListener(UnityAction listener)
    {
        freePrisonerListener.Add(listener);
        foreach (BulletControl bullet in freePrisonerInvoker)
        {
            bullet.AddFreePrisonerListener(listener);
        }
    }
}
