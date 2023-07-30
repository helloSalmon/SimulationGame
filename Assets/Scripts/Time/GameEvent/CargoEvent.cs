using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum CargoEventType
{
    Shipping,        //컨테이너 수령  
    Delivery      //컨테이너 배송
}

public abstract class CargoEvent : GameEvent
{
    public List<ContainerInfo> containers;      //스케줄에 관련된 컨테이너 목록
    public float startTime;                        //스케줄이 오는 시간
    public CargoEventType type;         //스케줄 이벤트 종류
    public int cargoCount;
    public bool active;

    public CargoEvent(CargoEventType cargoType, float startTime, int cargoCount,
                      CargoEventHandler eventHandler) : base(startTime, eventHandler)
    {
        this.startTime = startTime;
        this.cargoCount = cargoCount;
        containers = new List<ContainerInfo>();
        type = cargoType;
        active = false;
    }
}
