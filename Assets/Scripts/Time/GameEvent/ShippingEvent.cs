using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShippingEvent : CargoEvent
{
    public ShippingEvent(CargoEventType cargoType, float startTime, int cargoCount, CargoEventHandler eventHandler) :
        base(cargoType, startTime, cargoCount, eventHandler)
    {
        for (int n = 0; n < cargoCount; n++)
        {
            ContainerInfo currentInfo = new ContainerInfo();
            currentInfo.Code = Managers.Container.MakeCode();

            handler.Collection.containers.Add(currentInfo);
            containers.Add(currentInfo);
        }
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime && !handler.ContainerShip.gameObject.activeSelf)
        {
            if (!active)
            {
                // collection.deliveryCargoEvent.Add(this);
                active = true;
            }

            if (Managers.Time.scheduler.ship.currentShip == null)
            {
                if (handler.ShipInfo.waitingShips.Count > 0)
                {
                    handler.ShipInfo.currentShip = handler.ShipInfo.waitingShips[0];
                    handler.ShipInfo.waitingShips.RemoveAt(0);
                }
                
                handler.ContainerShip.EnterPort();
                HandleShippedContainers(handler.ContainerShip);
                Managers.Time.scheduler.ship.currentShip = this;
            }
        }
    }

    public void HandleShippedContainers(ContainerShip ship)
    {
        Debug.Log("Cargo is Landed");
        float containerOffset = 2.5f;

        //CargoEvent 안의 컨테이너들을 모두 생성해서 배치
        for (int i = 0; i < containers.Count; ++i)
        {
            //컨테이너 생성 후 배치 장소에 배치
            GameObject container = Managers.Container.CreateContainer(containers[i]);
            Vector3 offset = new Vector3(0, containerOffset, 0);
            container.transform.SetParent(handler.ContainerShip.containerLocations[i].gameObject.transform);
            container.transform.position = handler.ContainerShip.containerLocations[i].transform.position + offset;
            container.transform.rotation = Quaternion.LookRotation(Vector3.right);
            handler.ContainerShip.containerLocations[i].myContainer = container;

            Managers.Time.yardContainers.Add(container.GetComponent<Container>());
            Managers.Time.yardContainers[i].cargoEvent = this;
            containers[i].Real = container.GetComponent<Container>();
        }

        handler.Collection.shownCargoEvent.Remove(this);

        Unsubscribe();
    }
}

