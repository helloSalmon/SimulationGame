using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum CargoEventType
{
    Shipping,        //컨테이너 수령  
    Delivery      //컨테이너 배송
}

public class GameEvent
{
    public event Action<GameEvent> SignalRecieved;
    public float EventTriggerTime { get; set; }
    public LinkedListNode<GameEvent> Node { get; set; }
    private CargoEventHandler eventHandler;

    public GameEvent(float eventTriggerTime, CargoEventHandler eventHandler)
    {
        EventTriggerTime = eventTriggerTime;
        this.eventHandler = eventHandler;
    }

    public virtual void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime)
        {
            SignalRecieved?.Invoke(this);
        }
    }

    public void Unsubscribe()
    {
        eventHandler.cargoEvents.Remove(Node);
    }
}

public abstract class CargoEvent : GameEvent
{
    public List<IContainerInfo> containers;      //스케줄에 관련된 컨테이너 목록
    public float startTime;                        //스케줄이 오는 시간
    public CargoEventType type;         //스케줄 이벤트 종류
    public int cargoCount;
    public CargoEventCollection collection;
    public bool active;

    public CargoEvent(CargoEventType cargoType, float startTime, int cargoCount,
                      CargoEventCollection collection, CargoEventHandler eventHandler) : base(startTime, eventHandler)
    {
        this.startTime = startTime;
        this.cargoCount = cargoCount;
        this.collection = collection;
        containers = new List<IContainerInfo>();
        type = cargoType;
        active = false;
    }

    
}
