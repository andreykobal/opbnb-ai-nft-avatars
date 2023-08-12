using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static bool isApplicationPlaying;

    void Awake()
    {
        isApplicationPlaying = true;
    }

    void OnDestroy()
    {
        isApplicationPlaying = false;
    }

    public void Update()
    {
        while (_executionQueue.Count > 0)
        {
            _executionQueue.Dequeue().Invoke();
        }
    }

    public static void Enqueue(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        _executionQueue.Enqueue(action);
    }

    public static void RunOnMainThread(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        if (isApplicationPlaying)
        {
            Enqueue(action);
        }
        else
        {
            action.Invoke();
        }
    }
}