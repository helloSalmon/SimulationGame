using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryEvent : CargoEvent
{
    public DeliveryEvent(CargoEventType cargoType, float startTime, int cargoCount, ContainerCollection collection, CargoEventHandler eventHandler) :
        base(cargoType, startTime, cargoCount, collection, eventHandler)
    {
        IContainerInfo currentInfo = new ContainerInfo();

        //현재 컨테이너 야드에 있는 컨테이너를 배송 대상으로 지정할 때 
        if (collection.containers.Count != 0)
        {
            int rand = Random.Range(0, collection.containers.Count);
            currentInfo.Code = collection.containers[rand].Code;
            currentInfo.Size = collection.containers[rand].Size;
            collection.containers.RemoveAt(rand);
        }

        containers.Add(currentInfo);
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime)
        {
            if (!active)
            {
                collection.waitingCargoEvent.Add(this);
                collection.deliveryCargoEvent.Add(this);
                active = true;
            }

            foreach (ContainerLocation location in Managers.Time.containerHolderLocations)
            {
                // 컨테이너 홀더에 알맞은 컨테이너를 내려놓았을 때 컨테이너를 배송한다.
                if (location.myContainer != null && containers[0].Code == location.myContainer.GetComponent<TempContainer>().Code)
                {
                    HandleDeliveringContainers(location.myContainer, currentTime);
                    return;
                }
            }
        }
    }
    public void HandleDeliveringContainers(GameObject container, float gameTime)
    {
        Debug.Log("배송 완료된 컨테이너 번호 : " + containers[0].Code);
        ContainerCode.Remove(containers[0].Code);

        collection.containers.Remove(container.GetComponent<IContainerInfo>());
        collection.waitingCargoEvent.Remove(this);
        collection.deliveryCargoEvent.Remove(this);
        Object.Destroy(container);

        Score.Calculate(300, startTime, gameTime);

        Unsubscribe();
    }
}

