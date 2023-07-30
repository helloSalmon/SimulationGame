using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeliveryEvent : CargoEvent
{
    DeliveryHolder _holder;

    public DeliveryEvent(CargoEventType cargoType, float startTime, int cargoCount, CargoEventHandler eventHandler) :
        base(cargoType, startTime, cargoCount, eventHandler)
    {
        ContainerInfo currentInfo = new ContainerInfo();

        //현재 컨테이너 야드에 있는 컨테이너를 배송 대상으로 지정할 때 
        
        if (handler.Collection.containers.Count != 0)
        {
            int rand = Random.Range(0, handler.Collection.containers.Count);
            currentInfo.Code = handler.Collection.containers[rand].Code;
            handler.Collection.containers[rand].Real.cargoEvent = this;
            handler.Collection.containers.RemoveAt(rand);
        }

        containers.Add(currentInfo);
        _holder = eventHandler.AssignCodeToHolder(currentInfo.Code);
    }

    public override void CheckTrigger(float currentTime)
    {
        if (currentTime >= EventTriggerTime)
        {
            if (!active)
            {
                handler.Collection.shownCargoEvent.Add(this);
                handler.Collection.deliveryCargoEvent.Add(this);
                active = true;
            }

            if (_holder == null)
            {
                _holder = handler.AssignCodeToHolder(containers[0].Code);
                if (_holder == null)
                    return;
            }

            foreach (ContainerLocation location in Managers.Time.containerHolderLocations)
            {
                // 컨테이너 홀더에 알맞은 컨테이너를 내려놓았을 때 컨테이너를 배송한다.
                // sendlocation에 코드를 할당하는 이벤트를 만들어야 함.
                if (location.myContainer != null && containers[0].Code == location.myContainer.GetComponent<IContainerInfo>().Code
                    && location.gameObject.GetComponent<DeliveryHolder>().sendCode == _holder.sendCode)
                {
                    location.myContainer.GetComponent<CheckWorkRight>().CheckWorkCorrectly(1);
                    HandleDeliveringContainers(location.myContainer, currentTime);
                    return;
                }
            }
        }
    }
    private void HandleDeliveringContainers(GameObject container, float gameTime = 0f)
    {
        Debug.Log("배송 완료된 컨테이너 번호 : " + containers[0].Code);
        Managers.Container.RemoveContainer(container.GetComponent<Container>());
        Score.Calculate(300, startTime, gameTime);
        Unsubscribe();
    }

    public override void Unsubscribe()
    {
        handler.ClearCodeInHolder(_holder);
        handler.Collection.containers.Remove(containers[0]);
        handler.Collection.shownCargoEvent.Remove(this);
        handler.Collection.deliveryCargoEvent.Remove(this);
        base.Unsubscribe();
    }
}

