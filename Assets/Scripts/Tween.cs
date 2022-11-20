using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public static readonly List<Tween> QueuedTweens = new List<Tween>();
    public static readonly List<Tween> ActiveTweens = new List<Tween>();

    public delegate void InterpolationAction(float t);
    public delegate void CompletionCallback();

    private float startTime;
    private float duration;
    private InterpolationAction action;
    private CompletionCallback callback;

    private Tween(InterpolationAction action, float startTime, float duration, CompletionCallback callback=null)
    {
        this.action = action;
        this.startTime = startTime;
        this.duration = duration;
        this.callback = callback;
    }

    private bool Update()
    {
        if (duration <= 0) return false;
        float t = (Time.time - startTime) / duration;
        if (t < 1)
        {
            action.Invoke(t);
        }
        else
        {
            action.Invoke(1);
            callback?.Invoke();
        }
        return (t < 1);
    }

    public static void Start(InterpolationAction action, float duration, CompletionCallback callback = null)
    {
        QueuedTweens.Add(new Tween(action, Time.time, duration, callback));
    }

    public static void StartDelayedAction(float duration, CompletionCallback callback = null)
    {
        Start((t) => { }, duration, callback);
    }

    public static void UpdateAll()
    {
        // add queued tweens
        foreach (var t in QueuedTweens)
        {
            ActiveTweens.Add(t);
        }
        QueuedTweens.Clear();

        List<Tween> deadlist = new List<Tween>();
        foreach (var t in ActiveTweens)
        {
            // UPDATE
            bool alive = t.Update();

            // REMOVE
            if (!alive) deadlist.Add(t);
        }
        foreach (var t in deadlist)
        {
            ActiveTweens.Remove(t);
        }
    }
}
