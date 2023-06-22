using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

public partial class CargoEventHandler
{
    public LinkedList<GameEvent> cargoEvents;
    ShipList _ship;
    ContainerCollection _collection;
    float _currentTime;
    
    public CargoEventHandler(ShipList ship, ContainerCollection collection)
    {
        cargoEvents = new LinkedList<GameEvent>();
        _ship = ship;
        _collection = collection;
        Register(CheckShip, 0);
    }

    public void Register(Action<GameEvent> action, float startTime)
    {
        GameEvent ge = new GameEvent(startTime, this);
        ge.SignalRecieved += action;
        cargoEvents.AddFirst(ge);
        ge.Node = cargoEvents.First;
    }
    
    public void Register(CargoEventType type, float startTime, ContainerCollection collection)
    {
        GameEvent cargoEvent;
        switch (type)
        {
            case CargoEventType.Shipping:
                cargoEvent = new ShippingEvent(type, startTime, 3, collection, this);
                cargoEvents.AddFirst(cargoEvent);
                cargoEvent.Node = cargoEvents.First;
                collection.waitingCargoEvent.Add((CargoEvent)cargoEvent);
                break;
            case CargoEventType.Delivery:
                cargoEvent = new DeliveryEvent(type, startTime, 3, collection, this);
                cargoEvents.AddFirst(cargoEvent);
                cargoEvent.Node = cargoEvents.First;
                break;
        }
    }

    public void UpdateTime(float currentTime)
    {
        _currentTime = currentTime;
        var node = cargoEvents.First;
        var nextNode = node.Next;
        while (node != null)
        {
            nextNode = node.Next;
            node.Value.CheckTrigger(currentTime);
            node = nextNode;
        }
    }
}
