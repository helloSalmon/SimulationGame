using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CargoEventHandler
{
    public void CheckShip(GameEvent ge)
    {
        //대기하는 배가 있고 현재 배가 없으면 실행
        if (_ship.currentShip == null)
        {
            if (_ship.currentShip == null && _ship.waitingShips.Count != 0)
            {
                _ship.currentShip = _ship.waitingShips[0];
                _ship.waitingShips.RemoveAt(0);
                ShippingEvent se = (ShippingEvent)_ship.currentShip;
                se.HandleShippedContainers();

                //만약 더 이상 대기하는 배가 없으면 컨테이너 체크 함수 종료
                if (_ship.waitingShips == null)
                {
                    ge.Unsubscribe();
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

    public void GenerateDeliveryEvent(GameEvent ge)
    {
        if (_collection.deliveryCargoEvent.Count == 0 &&
            _collection.waitingCargoEvent.Count == 0)
        {
            ge.Unsubscribe();
            return;
        }
        if (_collection.containers.Count > 0 && _currentTime - ge.EventTriggerTime > 2.0f)
        {
            ge.EventTriggerTime = _currentTime;
            Register(CargoEventType.Delivery, _currentTime + Random.Range(0, 2), _collection);
        }
    }
}
