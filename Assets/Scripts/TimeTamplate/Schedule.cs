using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Schedule
{
    public float minTimeInterval = 5.0f;    //스케줄의 이벤트 간 최소 간격

    //스케줄 표
    public List<CargoEvent> shippingSchedules;    //컨테이너 배송 스케줄 목록
    public List<CargoEvent> waitingSchedules;       //대기중인 스케줄 목록
    public List<GameObject> loadedContainersInYard;       //현재 컨테이너 야드에 적재된 화물 목록
    public CargoEventType cargoEventType;                   //스케줄 이벤트 종류
    Image basicCalendar;

    public Schedule(Image basicCalendar)
    {
        shippingSchedules = new List<CargoEvent>();
        waitingSchedules = new List<CargoEvent>();
        loadedContainersInYard = new List<GameObject>();
        this.basicCalendar = basicCalendar;
    }

    //스케줄표 자동 생성
    public void CreateScheduleList()
    {
        int eventCount = 5;
        float currentTime = 0;
        string scheduleString = "스케줄 \n";
        //컨테이너 보내기 이벤트 생성 용 임시 컨테이너 리스트
        List<ContainerInfo> shippingContainers = new List<ContainerInfo>();

        //스케줄 간 최소 간격을 유지해서 생성
        for (int i = 0; i < eventCount; i++)
        {
            //발생 타이밍 결정
            currentTime += minTimeInterval + Random.Range(0.0f, 2.0f);

            //보내는 이벤트인지 받는 이벤트인지 결정
            if (Random.Range(1, 3) == 1 || shippingContainers.Count == 0)
            {
                cargoEventType = CargoEventType.Shipping;
            }
            else
            {
                cargoEventType = CargoEventType.Delievering;
            }

            //이벤트 생성
            CargoEvent creatingEvent = new CargoEvent(cargoEventType, currentTime, ref scheduleString, loadedContainersInYard, shippingContainers);

            //생성된 이벤트를 이벤트 리스트에 저장
            waitingSchedules.Add(creatingEvent);

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
        if (waitingSchedules.Count != 0 && waitingSchedules[0].startTime <= gameTime)
        {
            //시간이 됐으면 스케줄 실행 함수에 옮기기
            cargoEvent = waitingSchedules[0];

            //실행된 스케줄을 삭제
            waitingSchedules.RemoveAt(0);
        }
        return cargoEvent;
    }

    public bool CheckEndConditions(CargoEvent currentShip, int waitingCount)
    {
        //일과가 끝나는 조건이 모두 만족되면 시간 진행 종료
        if (waitingSchedules.Count == 0 && currentShip == null &&
            waitingCount == 0 && shippingSchedules.Count == 0)
        {
            Debug.Log("일과 종료!");
            return true;
        }
        return false;
    }
}
