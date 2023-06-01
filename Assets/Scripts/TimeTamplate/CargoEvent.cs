using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CargoEventType
{
    Shipping,        //컨테이너 수령  
    Delivery      //컨테이너 배송
}

public class ContainerCode
{
    static HashSet<string> _codeSet = new HashSet<string>();
    public static string Make()
    {
        string code = UnityEngine.Random.Range(1000, 9999).ToString();
        while (_codeSet.Contains(code))
            code = UnityEngine.Random.Range(1000, 9999).ToString();
        _codeSet.Add(code);
        return code;
    }
    public static void Remove(string code)
    {
        _codeSet.Remove(code);
    }
}

public class GameEvent
{
    public event Action SignalRecieved;
    public float EventTriggerTime { get; private set; }

    public GameEvent(float eventTriggerTime)
    {
        EventTriggerTime = eventTriggerTime;
    }

    public virtual void CheckTrigger(float currentTime)
    {
        if (currentTime > EventTriggerTime)
        {
            SignalRecieved?.Invoke();
        }
    }
}

public abstract class CargoEvent : GameEvent
{
    public List<ContainerInfo> containers;      //스케줄에 관련된 컨테이너 목록
    public float startTime;                        //스케줄이 오는 시간
    public CargoEventType type;         //스케줄 이벤트 종류
    public int cargoCount;

    public CargoEvent(CargoEventType cargoType, float startTime, int cargoCount) : base(startTime)
    {
        this.startTime = startTime;
        this.cargoCount = cargoCount;
        type = cargoType;
        containers = new List<ContainerInfo>();
    }
}
