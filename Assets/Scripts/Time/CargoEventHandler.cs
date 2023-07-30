using System;
using System.Collections.Generic;
using UnityEngine;

public partial class CargoEventHandler
{
    public LinkedList<GameEvent> cargoEvents = new LinkedList<GameEvent>();
    ShipInfo _shipInfo;
    CargoEventCollection _collection;
    ContainerShip _containerShip;
    CodeLabelSystem _codeLabelSystem;
    float _currentTime;

    public ShipInfo ShipInfo { get { return _shipInfo; } }
    public ContainerShip ContainerShip {  get { return _containerShip; } }
    public CargoEventCollection Collection { get { return _collection; } }
    
    public CargoEventHandler(ShipInfo ship, CargoEventCollection collection)
    {
        _shipInfo = ship;
        _collection = collection;
        _codeLabelSystem = GameObject.FindObjectOfType<CodeLabelSystem>();
        CreateContainerShip();
    }

    public void CreateContainerShip()
    {
        GameObject go = Managers.Resource.Instantiate("ContainerShip");
        _containerShip = go.GetComponent<ContainerShip>().init();
        _containerShip.gameObject.SetActive(false);
    }

    public void Register(Action<GameEvent> action, float startTime)
    {
        GameEvent ge = new GameEvent(startTime, this);
        ge.action += action;
        Enqueue(ge);
    }
    
    public void Register(CargoEventType type, float startTime, int cnt, CargoEventCollection collection)
    {
        GameEvent cargoEvent;
        switch (type)
        {
            case CargoEventType.Shipping:
                cargoEvent = new ShippingEvent(type, startTime, cnt, this);
                Enqueue(cargoEvent);
                collection.shownCargoEvent.Add((CargoEvent)cargoEvent);
                break;
            case CargoEventType.Delivery:
                cargoEvent = new DeliveryEvent(type, startTime, cnt, this);
                Enqueue(cargoEvent);
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

    public DeliveryHolder AssignCodeToHolder(string code)
    {
        DeliveryHolder holder = _codeLabelSystem.GetRandomHolder();
        if (holder == null)
        {
            return null;
        }
        _codeLabelSystem.AssignHolder(holder, code);
        return holder;
    }

    public void ClearCodeInHolder(DeliveryHolder holder)
    {
        _codeLabelSystem.ClearHolder(holder);
    }

    private void Enqueue(GameEvent cargoEvent)
    {
        cargoEvents.AddFirst(cargoEvent);
        cargoEvent.Node = cargoEvents.First;
    }
}
