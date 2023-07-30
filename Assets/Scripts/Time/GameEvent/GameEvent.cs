using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEvent
{
    public event Action<GameEvent> action;
    public float EventTriggerTime { get; set; }
    public LinkedListNode<GameEvent> Node { get; set; }
    protected CargoEventHandler handler;

    public GameEvent(float eventTriggerTime, CargoEventHandler eventHandler)
    {
        EventTriggerTime = eventTriggerTime;
        handler = eventHandler;
    }

    public virtual void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime)
        {
            action?.Invoke(this);
        }
    }

    public virtual void Unsubscribe()
    {
        handler.cargoEvents.Remove(Node);
    }
}
