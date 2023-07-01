using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CargoEventHandler
{
    /// <summary>
    /// 배에 있는 화물을 전부 내리면 컨테이너 선을 내보낸다.
    /// </summary>
    public void CheckShip(GameEvent ge)
    {
        //대기하는 배가 있고 현재 배가 없으면 실행
        if (_shipInfo.currentShip == null)
        {
            if (_shipInfo.waitingShips.Count == 0 && _collection.deliveryCargoEvent.Count == 0 && _collection.shownCargoEvent.Count == 0)
            {
                //만약 더 이상 대기하는 배가 없으면 컨테이너 체크 함수 종료
                ge.Unsubscribe();
            }
            return;
        }

        int clearCount = 0;

        foreach (ContainerLocation location in _containerShip.containerLocations)
        {
            if (location.myContainer == null)
                clearCount++;
        }

        if (clearCount == _containerShip.containerLocations.Count)
        {
            Score.Calculate(100, _shipInfo.currentShip.startTime, Managers.Time.gameTime);
            _shipInfo.currentShip = null;
            Debug.Log("Ship is Clear");
            _containerShip.ExitPort();
        }
    }

    public void GenerateDeliveryEvent(GameEvent ge)
    {
        if (_collection.deliveryCargoEvent.Count == 0 && _shipInfo.currentShip == null && _shipInfo.waitingShips.Count == 0 && _collection.shownCargoEvent.Count == 0)
        {
            ge.Unsubscribe();
            return;
        }
        if (_collection.containers.Count > 0 && _currentTime - ge.EventTriggerTime > 2.0f)
        {
            ge.EventTriggerTime = _currentTime;
            Register(CargoEventType.Delivery, _currentTime + Random.Range(0, 2), 1, _collection);
        }
    }
}
