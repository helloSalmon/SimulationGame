using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoEventCollection
{
    /// <summary>
    /// 컨테이너 야드에 올라와 있는 배송 이벤트 목록
    /// </summary>
    public List<CargoEvent> deliveryCargoEvent;
    /// <summary>
    /// 보여지는 운송, 배송 이벤트 목록
    /// </summary>
    public List<CargoEvent> shownCargoEvent;
    /// <summary>
    /// 현재 모든 운송, 배송 스케줄에 포함된 모든 컨테이너 정보 목록
    /// </summary>
    public List<ContainerInfo> containers;

    public CargoEventCollection()
    {
        deliveryCargoEvent = new List<CargoEvent>();
        shownCargoEvent = new List<CargoEvent>();
        containers = new List<ContainerInfo>();
    }
}
