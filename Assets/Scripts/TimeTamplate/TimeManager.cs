using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 컨테이너가 구현해야 하는 메소드
public interface IContainerInfo
{
    public string Code { get; set; }
    public Vector2 Size { get; set; }
}
// 컨테이너 정보
public class ContainerInfo : IContainerInfo
{
    public string Code { get; set; }
    public Vector2 Size { get; set; }
}
public class Ship
{
    public CargoEvent currentShip;                                   //현재 컨테이너를 내리고 있는 화물선 (현재 진행 중인 컨테이너 수령 이벤트)
    public List<CargoEvent> waitingShips;                            //뒤에서 대기 중인 화물선 (대기 중인 컨테이너 수령 이벤트)

    public Ship()
    {
        currentShip = null;
        waitingShips = new List<CargoEvent>();
    }
}

public class TimeManager : MonoBehaviour
{
    //시간 관련 변수들
    public float gameTime;                      //현재 일과 중 흘러가는 시간
    public float timeSpeed = 1.0f;            //시간 배속
    public float maxTime;                       //현재 일과의 종료 시간
    public float permittedEventTime;                  //허용될 수 있는 스케줄 상 이벤트 수행 시간

    //컨테이너 관련 변수들
    public GameObject basicContainer;                           //기본 컨테이너
    public List<ContainerLocation> containerSpawnLocations;         //컨테이너가 생성되는 위치
    public List<ContainerLocation> containerHolderLocations;       //보낼 컨테이너를 저장할 홀더들 (옮겨진 컨테이너의 정보를 확인할 스크립트 필요)

    private Schedule _schedule;
    private Ship _ship;

    //UI
    public Image basicCalendar;      //스케줄 표 생성에 사용되는 기본 스케줄 표 양식
    public Text timeText;                                   //현재 시각을 표시하는 텍스트
    public Image scheduleCalendar;             //유저가 보는 스케줄 표

    private void Start()
    {
        _ship = new Ship();
        _schedule = new Schedule(basicCalendar);
        _schedule.CreateScheduleList();
        
        StartCoroutine(TimeGoing());
        StartCoroutine(CheckShipsWaiting());

        Score.currentScore = 0;
    }

    //일과 진행의 주축이 되는 함수
    private IEnumerator TimeGoing()
    {
        while (!_schedule.CheckEndConditions(_ship.currentShip, _ship.waitingShips.Count))
        {
            //배속에 맞게 시간을 돌림
            gameTime += Time.deltaTime * timeSpeed;

            //mainTime을 60진법에 맞게 변환 (게임 시간 내 속도로 변환) 
            timeText.text = "현재 시각 : \n" + gameTime.ToString();

            DetermineSchedule();
            DeliverCargo();

            yield return null;
        }
    }

    
    private void DetermineSchedule()
    {
        CargoEvent currentCargoEvent = _schedule.GetFirstSchedule(gameTime);
        if (currentCargoEvent == null)
            return;
        
        //스케줄 종류를 구분하고 그에 맞는 함수 실행
        switch (currentCargoEvent.type)
        {
            case CargoEventType.Shipping:
                if (_ship.currentShip == null)
                {
                    HandleShippedContainers(currentCargoEvent);
                    _ship.currentShip = currentCargoEvent;
                }
                else
                {
                    _ship.waitingShips.Add(currentCargoEvent);
                    StartCoroutine(CheckShipIsEmpty());
                    Debug.Log("Ship is Waiting");
                }
                break;

            case CargoEventType.Delievering:
                _schedule.shippingSchedules.Add(currentCargoEvent);
                Debug.Log("배송 대기 중인 컨테이너 번호 : " + currentCargoEvent.containers[0].Code);
                break;
        }
    }

    private void DeliverCargo()
    {
        //화물을 보낼 수 있는 지 확인하는 조건문
        if (_schedule.shippingSchedules.Count == 0)
            return;

        foreach (CargoEvent events in _schedule.shippingSchedules)
        {
            foreach (ContainerLocation location in containerHolderLocations)
            {
                //보낼 컨테이너와 하역장에 내려진 화물이 일치하면 해당 화물 전송
                if (location.myContainer != null && events.containers[0].Code == location.myContainer.GetComponent<TempContainer>().Code)
                {
                    HandleDeliveringContainers(location.myContainer, events);
                    return;
                }
                else if (location.myContainer != null && events.containers[0].Code != location.myContainer.GetComponent<TempContainer>().Code)
                {
                    Debug.Log("잘못된 컨테이너를 내렸습니다");
                    return;
                }
            }
        }
    }

    //대기하고 있는 배(화물선)가 있는 경우 돌아가는 함수
    private IEnumerator CheckShipsWaiting()
    {
        while (true)
        {
            //대기하는 배가 있고 현재 배가 없으면 실행
            if (_ship.currentShip == null && _ship.waitingShips.Count != 0)
            {
                _ship.currentShip = _ship.waitingShips[0];
                _ship.waitingShips.RemoveAt(0);
                HandleShippedContainers(_ship.currentShip);

                //만약 더 이상 대기하는 배가 없으면 컨테이너 체크 함수 종료
                if (_ship.waitingShips == null)
                {
                    StopCoroutine(CheckShipIsEmpty());
                }
            }
            yield return null;
        }
    }

    //컨테이너 다 내렸는지 확인하는 함수
    private IEnumerator CheckShipIsEmpty()
    {
        while (true)
        {
            int clearCount = 0;

            foreach (ContainerLocation location in containerSpawnLocations)
            {
                if (location.myContainer == null)
                    clearCount++;
            }

            if (clearCount == containerSpawnLocations.Count)
            {
                Score.Calculate(100, _ship.currentShip.startTime, gameTime, permittedEventTime);
                _ship.currentShip = null;
                Debug.Log("Ship is Clear");
            }

            yield return null;
        }
    }

    //화물 수령 이벤트 함수
    public void HandleShippedContainers(CargoEvent cargoEvent)
    {
        Debug.Log("Cargo is Landed");

        int count = 0;

        //CargoEvent 안의 컨테이너들을 모두 생성해서 배치
        foreach (IContainerInfo containerInfo in cargoEvent.containers)
        {
            //컨테이너 생성 후 배치 장소에 배치
            GameObject container = CreateContainer(containerInfo);
            Vector3 offset = new Vector3(0, container.transform.localScale.y / 2, 0);
            container.transform.position = containerSpawnLocations[count].transform.position + offset;
            containerSpawnLocations[count].myContainer = container;

            _schedule.loadedContainersInYard.Add(container);

            count++;
        }
    }
    
    //화물 배송 이벤트 함수 (실제 배송)
    public void HandleDeliveringContainers(GameObject container, CargoEvent cargoEvent)
    {
        Debug.Log("배송 완료된 컨테이너 번호 : " + cargoEvent.containers[0].Code);

        //현재 컨테이너 목록에서 삭제
        _schedule.loadedContainersInYard.Remove(container);
        _schedule.shippingSchedules.Remove(cargoEvent);

        //컨테이너 완전 삭제
        Destroy(container);

        Score.Calculate(300, cargoEvent.startTime, gameTime, permittedEventTime);
    }

    //컨테이너(화물) 생성 함수
    public GameObject CreateContainer(IContainerInfo containerInfo)
    {
        TempContainer tempContainer = ResourceManager.Instance.Instantiate("Container").GetComponent<TempContainer>();

        //g에 컨테이너 정보 입력 및 컨테이너 외형 생성
        tempContainer.Code = containerInfo.Code;
        tempContainer.Size = containerInfo.Size;

        return tempContainer.gameObject;
    }
}
