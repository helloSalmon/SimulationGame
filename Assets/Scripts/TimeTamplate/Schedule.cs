using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoEventCollection
{
    public List<CargoEvent> deliveryCargoEvent;    //컨테이너 배송 스케줄 목록
    public List<CargoEvent> shownCargoEvent;       //대기중인 스케줄 목록
    public List<IContainerInfo> containers;       //현재 컨테이너 야드에 적재된 화물 목록

    public CargoEventCollection()
    {
        deliveryCargoEvent = new List<CargoEvent>();
        shownCargoEvent = new List<CargoEvent>();
        containers = new List<IContainerInfo>();
    }
}

public class Schedule
{
    public float minTimeInterval = 5.0f;    //스케줄의 이벤트 간 최소 간격
    public CargoEventCollection collection;
    public CargoEventHandler cargoEventHandler;
    public ShipInfo ship;
    public float gameEndTime = 0.0f;
    Image basicCalendar;
    List<Image> calendars;
    List<Text> texts;

    public Schedule(Image basicCalendar)
    {
        ship = new ShipInfo();
        collection = new CargoEventCollection();
        cargoEventHandler = new CargoEventHandler(ship, collection);
        calendars = new List<Image>();
        texts = new List<Text>();
        this.basicCalendar = basicCalendar;
    }

    //스케줄표 자동 생성
    public void CreateScheduleList(int eventCount, float gameEnd, int numEvents)
    {
        float currentTime = 0;
        
        //컨테이너 보내기 이벤트 생성 용 임시 컨테이너 리스트

        //스케줄 간 최소 간격을 유지해서 생성
        for (int i = 0; i < eventCount; i++)
        {
            //발생 타이밍 결정
            currentTime += minTimeInterval + Random.Range(0.0f, 2.0f);
            cargoEventHandler.Register(CargoEventType.Shipping, currentTime, numEvents, collection);
        }

        cargoEventHandler.Register(cargoEventHandler.GenerateDeliveryEvent, currentTime);

        gameEndTime = currentTime + gameEnd;
    }

    public void MakeScheduleString()
    {
        string containerId;
        string scheduleString;
        for (int i = 0; i < collection.shownCargoEvent.Count; i++)
        {
            scheduleString = "작업 시간 : " + collection.shownCargoEvent[i].startTime.ToString() + "\n";
            scheduleString += "작업 종류 : ";
            //만약 화물 받기면 화물 생성
            if (collection.shownCargoEvent[i].type == CargoEventType.Shipping)
            {
                scheduleString += "화물 수령 \n";
                scheduleString += "받는 컨테이너의 코드 목록 \n";

                for (int j = 0; j < collection.shownCargoEvent[i].containers.Count; j++)
                {
                    containerId = collection.shownCargoEvent[i].containers[j].Code;
                    scheduleString += Managers.Container.GetRegularCode(containerId) + " /";
                    if (j % 3 == 2) scheduleString += "\n";
                }
            }
            //만약 화물 보내기면 있는 화물 중에서 선택
            else if (collection.shownCargoEvent[i].type == CargoEventType.Delivery)
            {
                scheduleString += "화물 배송 \n";
                scheduleString += "보낼 컨테이너 코드 : ";
                containerId = collection.shownCargoEvent[i].containers[0].Code;
                scheduleString += Managers.Container.GetRegularCode(containerId);

            }
            //유저가 인게임에서 볼 수 있는 형태의 스케줄표 표현
            if (i == calendars.Count)
            {
                calendars.Add(Object.Instantiate(basicCalendar));
                texts.Add(calendars[i].GetComponentInChildren<Text>());
                calendars[i].transform.SetParent(basicCalendar.transform.parent.GetChild(0), false);
            }
            texts[i].text = scheduleString;
            // calendars[i].rectTransform.anchorMin = new Vector2(0.5f, 1);
            // calendars[i].rectTransform.anchorMax = new Vector2(0.5f, 1);
            calendars[i].transform.localPosition = new Vector3(0, -i * calendars[i].rectTransform.sizeDelta.y, 0);
        }
        for (int i = calendars.Count - 1; i >= collection.shownCargoEvent.Count; i--)
        {
            calendars[i].transform.SetParent(null);
            Object.Destroy(texts[i]);
            Object.Destroy(calendars[i]);
            texts.RemoveAt(i);
            calendars.RemoveAt(i);
        }
    }

    public void ReflectRestScore(float gameTime)
    {
        foreach (var ce in collection.deliveryCargoEvent)
        {
            Score.Calculate(300, ce.EventTriggerTime, gameTime);
        }

        foreach (var ce in ship.waitingShips)
        {
            Score.Calculate(300 * ce.containers.Count, ce.EventTriggerTime + Score.permittedEventTime, gameTime);
        }
    }
    
    public bool CheckEndConditions(float gameTime)
    {
        if (gameEndTime < gameTime) return true;
        //일과가 끝나는 조건이 모두 만족되면 시간 진행 종료
        if (collection.shownCargoEvent.Count == 0 && ship.currentShip == null &&
            ship.waitingShips.Count == 0 && collection.deliveryCargoEvent.Count == 0)
        {
            return true;
        }
        return false;
    }
}
