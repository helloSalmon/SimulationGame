using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Schedule
{
    float eventTypeP = 0.33f;

    public float minTimeInterval = 5.0f;    //스케줄의 이벤트 간 최소 간격

    //스케줄 표
    public List<CargoEvent> deliveryCargoEvent;    //컨테이너 배송 스케줄 목록
    public List<CargoEvent> waitingCargoEvent;       //대기중인 스케줄 목록
    public List<IContainerInfo> containersInYard;       //현재 컨테이너 야드에 적재된 화물 목록
    public CargoEventHandler cargoEventHandler;
    Image basicCalendar;

    public Schedule(Image basicCalendar)
    {
        deliveryCargoEvent = new List<CargoEvent>();
        waitingCargoEvent = new List<CargoEvent>();
        containersInYard = new List<IContainerInfo>();
        cargoEventHandler = new CargoEventHandler();
        this.basicCalendar = basicCalendar;
    }

    //스케줄표 자동 생성
    public void CreateScheduleList(int eventCount)
    {
        float currentTime = 0;
        
        //컨테이너 보내기 이벤트 생성 용 임시 컨테이너 리스트
        List<ContainerInfo> containersInShip = new List<ContainerInfo>();

        //스케줄 간 최소 간격을 유지해서 생성
        for (int i = 0; i < eventCount; i++)
        {
            //발생 타이밍 결정
            currentTime += minTimeInterval + Random.Range(0.0f, 2.0f);

            //보내는 이벤트인지 받는 이벤트인지 결정
            if (Random.value <= eventTypeP || containersInShip.Count == 0)
            {
                cargoEventHandler.Register(CargoEventType.Shipping, currentTime, containersInYard, containersInShip);
            }
            else
            {
                cargoEventHandler.Register(CargoEventType.Delivery, currentTime, containersInYard, containersInShip);
            }
        }
    }

    public void MakeScheduleString()
    {
        
        string scheduleString = "스케줄 \n";
        for (int i = 0; i < waitingCargoEvent.Count; i++)
        {
            scheduleString = "작업 시간 : " + waitingCargoEvent[i].startTime.ToString() + "\n";
            scheduleString += "작업 종류 : ";
            //만약 화물 받기면 화물 생성
            if (waitingCargoEvent[i].type == CargoEventType.Shipping)
            {
                scheduleString += "화물 수령 \n";
                scheduleString += "받는 컨테이너의 코드 목록 \n";

                for (int j = 0; j < waitingCargoEvent[i].containers.Count; j++)
                {
                    scheduleString += waitingCargoEvent[i].containers[j].Code.ToString() + " / ";
                }
            }
            //만약 화물 보내기면 있는 화물 중에서 선택
            else if (waitingCargoEvent[i].type == CargoEventType.Delivery)
            {
                scheduleString += "화물 배송 \n";
                scheduleString += "보낼 컨테이너 코드 : ";
                scheduleString += waitingCargoEvent[i].containers[0].Code.ToString();

            }

            Image currentCalendar = Object.Instantiate(basicCalendar);
            //유저가 인게임에서 볼 수 있는 형태의 스케줄표 표현
            Text currentScheduleText = currentCalendar.GetComponentInChildren<Text>();

            currentScheduleText.text = scheduleString;
            currentCalendar.transform.SetParent(basicCalendar.transform.parent.GetChild(0), false);
            currentCalendar.transform.localPosition = new Vector3(0, -i * currentCalendar.rectTransform.sizeDelta.y, 0);
        }
    }

    public CargoEvent GetFirstSchedule(float gameTime)
    {
        CargoEvent cargoEvent = null;
        //scheduleList의 첫 번째 항목이 나올 시간이 되었는지 체크
        if (waitingCargoEvent.Count != 0 && waitingCargoEvent[0].startTime <= gameTime)
        {
            //시간이 됐으면 스케줄 실행 함수에 옮기기
            cargoEvent = waitingCargoEvent[0];

            //실행된 스케줄을 삭제
            waitingCargoEvent.RemoveAt(0);
        }
        return cargoEvent;
    }

    public bool CheckEndConditions(CargoEvent currentShip, int waitingCount)
    {
        //일과가 끝나는 조건이 모두 만족되면 시간 진행 종료
        if (waitingCargoEvent.Count == 0 && currentShip == null &&
            waitingCount == 0 && deliveryCargoEvent.Count == 0)
        {
            Debug.Log("일과 종료!");
            return true;
        }
        return false;
    }
}
