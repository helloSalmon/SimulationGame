using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShippingEvent : CargoEvent
{
    public ShippingEvent(CargoEventType cargoType, float startTime, int cargoCount, List<IContainerInfo> containersInYard, List<ContainerInfo> containersInShip) :
        base(cargoType, startTime, cargoCount)
    {
        for (int n = 0; n < cargoCount; n++)
        {
            ContainerInfo currentInfo = new ContainerInfo();
            currentInfo.Code = ContainerCode.Make(); //중복되는 코드 생성하지 않는 기능 필요
            currentInfo.Size = new Vector2(2, 1);

            //임시 리스트에 추가
            containersInShip.Add(currentInfo);

            containers.Add(currentInfo);
        }
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= startTime)
        {
            if (Managers.Time.ship.currentShip == null)
            {
                HandleShippedContainers();
                Managers.Time.ship.currentShip = this;
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
            GameObject container = Managers.Time.CreateContainer(containers[i]);
            Vector3 offset = new Vector3(0, container.transform.localScale.y / 2, 0);

            container.transform.position = Managers.Time.containerSpawnLocations[i].transform.position + offset;
            Managers.Time.containerSpawnLocations[i].myContainer = container;
            Managers.Time.scheduler.containersInYard.Add(container.GetComponent<IContainerInfo>());

        }

        Managers.Time.scheduler.cargoEventHandler.cargoEvents.Remove(this);
    }
}

