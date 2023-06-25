using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShippingEvent : CargoEvent
{
    public ShippingEvent(CargoEventType cargoType, float startTime, int cargoCount, CargoEventCollection collection, CargoEventHandler eventHandler) :
        base(cargoType, startTime, cargoCount, collection, eventHandler)
    {
        for (int n = 0; n < cargoCount; n++)
        {
            ContainerInfo currentInfo = new ContainerInfo();
            currentInfo.Code = Managers.Container.MakeCode(); //중복되는 코드 생성하지 않는 기능 필요
            currentInfo.Size = new Vector2(2, 1);

            //임시 리스트에 추가
            collection.containers.Add(currentInfo);
            containers.Add(currentInfo);
        }
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime)
        {
            if (!active)
            {
                collection.deliveryCargoEvent.Add(this);
                active = true;
            }

            if (Managers.Time.scheduler.ship.currentShip == null)
            {
                HandleShippedContainers();
                Managers.Time.scheduler.ship.currentShip = this;
            }
        }
    }

    public void HandleShippedContainers()
    {
        Debug.Log("Cargo is Landed");

        //CargoEvent 안의 컨테이너들을 모두 생성해서 배치
        for (int i = 0; i < containers.Count; ++i)
        {
            //컨테이너 생성 후 배치 장소에 배치
            GameObject container = Managers.Container.CreateContainer(containers[i]);
            Vector3 offset = new Vector3(0, container.transform.localScale.y / 2, 0);
            container.transform.position = Managers.Time.containerSpawnLocations[i].transform.position + offset;

            Managers.Time.containerSpawnLocations[i].myContainer = container;
        }

        collection.waitingCargoEvent.Remove(this);

        Unsubscribe();
    }
}

