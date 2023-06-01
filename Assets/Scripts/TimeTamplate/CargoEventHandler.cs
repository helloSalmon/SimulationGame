using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

public class CargoEventHandler
{
    public LinkedList<GameEvent> cargoEvents;
    private ShipList _ship;
    GameEvent checkShip;

    public CargoEventHandler(ShipList ship)
    {
        cargoEvents = new LinkedList<GameEvent>();
        checkShip = new GameEvent(0);
        checkShip.SignalRecieved += CheckShip;
        cargoEvents.AddFirst(checkShip);
        _ship = ship;
    }

    private void CheckShip() {
        //대기하는 배가 있고 현재 배가 없으면 실행

        if (_ship.currentShip == null)
        {
            if (_ship.currentShip == null && _ship.waitingShips.Count != 0)
            {
                _ship.currentShip = _ship.waitingShips[0];
                _ship.waitingShips.RemoveAt(0);
                ShippingEvent e = (ShippingEvent) _ship.currentShip;
                e.HandleShippedContainers();

                //만약 더 이상 대기하는 배가 없으면 컨테이너 체크 함수 종료
                if (_ship.waitingShips == null)
                {
                    cargoEvents.Remove(checkShip);
                }
            }
            return;
        }

        int clearCount = 0;

        foreach (ContainerLocation location in Managers.Time.containerSpawnLocations)
        {
            if (location.myContainer == null)
                clearCount++;
        }

        if (clearCount == Managers.Time.containerSpawnLocations.Count)
        {
            Score.Calculate(100, _ship.currentShip.startTime, Managers.Time.gameTime);
            _ship.currentShip = null;
            Debug.Log("Ship is Clear");
        }
    }

    public void Register(CargoEventType type, float startTime, List<IContainerInfo> containersInYard, List<ContainerInfo> containersInShip)
    {
        GameEvent cargoEvent;
        switch (type)
        {
            case CargoEventType.Shipping:
                cargoEvent = new ShippingEvent(type, startTime, 3, containersInYard, containersInShip);
                cargoEvents.AddFirst(cargoEvent);
                Managers.Time.scheduler.waitingCargoEvent.Add((CargoEvent)cargoEvent);
                break;
            case CargoEventType.Delivery:
                cargoEvent = new DeliveryEvent(type, startTime, 3, containersInYard, containersInShip);
                cargoEvents.AddFirst(cargoEvent);
                Managers.Time.scheduler.waitingCargoEvent.Add((CargoEvent)cargoEvent);
                break;
        }
    }

    public void UpdateTime(float currentTime)
    {
        for (var node = cargoEvents.First; node != null;)
        {
            var nextNode = node.Next;
            node.Value.CheckTrigger(currentTime);
            node = nextNode;
        }
    }
}
