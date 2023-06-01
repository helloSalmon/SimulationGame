using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryEvent : CargoEvent
{
    float chooseP = 0.33f;

    public DeliveryEvent(CargoEventType cargoType, float startTime, int cargoCount, List<IContainerInfo> containersInYard, List<ContainerInfo> containersInShip) :
        base(cargoType, startTime, cargoCount)
    {
        ContainerInfo currentInfo = new ContainerInfo();

        //현재 컨테이너 야드에 있는 컨테이너를 배송 대상으로 지정할 때 
        if (containersInYard.Count != 0 && Random.value <= chooseP)
        {
            int rand = Random.Range(0, containersInYard.Count);
            currentInfo.Code = containersInYard[rand].Code;
            currentInfo.Size = containersInYard[rand].Size;
        }
        //오늘 올 컨테이너를 배송 대상으로 지정할 때
        else
        {
            currentInfo = containersInShip[Random.Range(0, containersInShip.Count)];
        }

        containers.Add(currentInfo);
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= startTime)
        {
            //화물을 보낼 수 있는 지 확인하는 조건문
            //if (Managers.Time.scheduler.deliveryCargoEvent.Count == 0)
            //    return;

            foreach (ContainerLocation location in Managers.Time.containerHolderLocations)
            {
                // 컨테이너 홀더에 알맞은 컨테이너를 내려놓았을 때 컨테이너를 배송한다.
                if (location.myContainer != null && containers[0].Code == location.myContainer.GetComponent<TempContainer>().Code)
                {
                    HandleDeliveringContainers(location.myContainer, currentTime);
                    return;
                }
                else if (location.myContainer != null && containers[0].Code != location.myContainer.GetComponent<TempContainer>().Code)
                {
                    Debug.Log("잘못된 컨테이너를 내렸습니다");
                }
            }
        }
    }
    public void HandleDeliveringContainers(GameObject container, float gameTime)
    {
        Debug.Log("배송 완료된 컨테이너 번호 : " + containers[0].Code);
        ContainerCode.Remove(containers[0].Code);

        //현재 컨테이너 목록에서 삭제
        Managers.Time.scheduler.containersInYard.Remove(container.GetComponent<IContainerInfo>());
        Managers.Time.scheduler.deliveryCargoEvent.Remove(this);

        //컨테이너 완전 삭제
        UnityEngine.Object.Destroy(container);

        Score.Calculate(300, startTime, gameTime);
        Managers.Time.scheduler.cargoEventHandler.cargoEvents.Remove(this);
    }
}

