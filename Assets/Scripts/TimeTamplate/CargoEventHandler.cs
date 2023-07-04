using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

public partial class CargoEventHandler
{
    public LinkedList<GameEvent> cargoEvents = new LinkedList<GameEvent>();
    ShipInfo _shipInfo;
    CargoEventCollection _collection;
    ContainerShip _containerShip;
    float _currentTime;

    public ShipInfo Ship { get { return _shipInfo; } }
    
    public CargoEventHandler(ShipInfo ship, CargoEventCollection collection)
    {
        _shipInfo = ship;
        _collection = collection;
        Register(CheckShip, 0);
        CreateContainerShip();
    }

    public void CreateContainerShip()
    {
        GameObject go = Managers.Resource.Instantiate("ContainerShip");
        _containerShip = go.GetComponent<ContainerShip>().init();
        _containerShip.gameObject.SetActive(false);
    }

    public ContainerShip GetContainerShip()
    {
        return _containerShip;
    }

    public void Register(Action<GameEvent> action, float startTime)
    {
        GameEvent ge = new GameEvent(startTime, this);
        ge.SignalRecieved += action;
        cargoEvents.AddFirst(ge);
        ge.Node = cargoEvents.First;
    }
    
    public void Register(CargoEventType type, float startTime, int cnt, CargoEventCollection collection)
    {
        GameEvent cargoEvent;
        switch (type)
        {
            case CargoEventType.Shipping:
                cargoEvent = new ShippingEvent(type, startTime, cnt, collection, this);
                cargoEvents.AddFirst(cargoEvent);
                cargoEvent.Node = cargoEvents.First;
                collection.shownCargoEvent.Add((CargoEvent)cargoEvent);
                break;
            case CargoEventType.Delivery:
                cargoEvent = new DeliveryEvent(type, startTime, cnt, collection, this);
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
