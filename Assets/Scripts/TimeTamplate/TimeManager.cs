using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    //컨테이너 정보
    public class ContainerInfo
    {
        public string code;                             //컨테이너 코드번호
        public Vector2 size;                            //컨테이너 크기
    }

    //스케줄 클래스
    public class CargoEvent
    {
        public List<ContainerInfo> containers;      //스케줄에 관련된 컨테이너 목록
        public float launchTime;                        //스케줄이 오는 시간
        public TodayEventsType eventType;         //스케줄 이벤트 종류
    }
    
    //스케줄 이벤트 종류
    public enum TodayEventsType
    {
        getCargo,        //컨테이너 수령  
        sendCargo      //컨테이너 배송
    }
    private TodayEventsType todayEventsType;
    
    //시간 관련 변수들
    public float mainTime;                      //현재 일과 중 흘러가는 시간
    public float timeSpeed = 1.0f;            //시간 배속
    public float minTimeDistance = 5.0f;    //스케줄의 이벤트 간 최소 간격
    public float maxTime;                       //현재 일과의 종료 시간
    public float possibleTime;                  //허용될 수 있는 스케줄 상 이벤트 수행 시간

    //스케줄 표
    public List<CargoEvent> schaduleList = new();       //대기중인 스케줄 목록
    public List<CargoEvent> willSendCargo = new();    //컨테이너 배송 스케줄 목록

    //컨테이너 관련 변수들
    public GameObject basicContainer;                           //기본 컨테이너
    public List<ContainerLocation> getContainerLoc;         //컨테이너가 생성되는 위치
    public List<ContainerLocation> sendContainerLoc;       //보낼 컨테이너를 저장할 홀더들 (옮겨진 컨테이너의 정보를 확인할 스크립트 필요)
    public List<GameObject> nowContainers = new();       //현재 컨테이너 야드에 적재된 화물 목록
    public CargoEvent nowShip;                                   //현재 컨테이너를 내리고 있는 화물선 (현재 진행 중인 컨테이너 수령 이벤트)
    public List<CargoEvent> waitingShips = new();           //뒤에서 대기 중인 화물선 (대기 중인 컨테이너 수령 이벤트)

    //점수 관련 변수들
    private int nowScore;        //현재 일과의 점수

    //UI
    public Text timeText;                                   //현재 시각을 표시하는 텍스트
    public GameObject schaduleCalendar;             //유저가 보는 스케줄 표
    public UnityEngine.UI.Image basicCalendar;      //스케줄 표 생성에 사용되는 기본 스케줄 표 양식

    private void Start()
    {
        CreateSchadule();
        
        StartCoroutine(TimeGoing());
        StartCoroutine(WaitShip());

        nowScore = 0;
    }

    //일과 진행의 주축이 되는 함수
    private IEnumerator TimeGoing()
    {
        while (true)
        {
            //일과가 끝나는 조건이 모두 만족되면 시간 진행 종료
            if (schaduleList.Count == 0 && nowShip == null && 
                waitingShips.Count == 0 && willSendCargo.Count == 0)
            {
                Debug.Log("일과 종료!");

                break;
            }
            
            //배속에 맞게 시간을 돌림
            mainTime += Time.deltaTime * timeSpeed;

            //mainTime을 60진법에 맞게 변환 (게임 시간 내 속도로 변환) 
            timeText.text = "현재 시각 : " + mainTime.ToString();

            //schaduleList의 첫 번째 항목이 나올 시간이 되었는지 체크
            if (schaduleList.Count != 0 && schaduleList[0].launchTime <= mainTime)
            {
                //시간이 됐으면 스케줄 실행 함수에 옮기기
                CargoEvent nowEventCargos = schaduleList[0];

                //실행된 스케줄을 삭제
                schaduleList.RemoveAt(0);

                //스케줄 종류를 구분하고 그에 맞는 함수 실행
                switch (nowEventCargos.eventType)
                {
                    case TodayEventsType.getCargo:
                        if (nowShip == null)
                        {
                            GetCargoEvent(nowEventCargos);
                            nowShip = nowEventCargos;
                        }
                        else
                        {
                            waitingShips.Add(nowEventCargos);
                            StartCoroutine(CheckGetEnd());
                            Debug.Log("Ship is Waiting");
                        }
                        break;

                    case TodayEventsType.sendCargo:
                        willSendCargo.Add(nowEventCargos);
                        Debug.Log("배송 대기 중인 컨테이너 번호 : " + nowEventCargos.containers[0].code);
                        break;
                }
            }
            yield return null;

            //화물을 보낼 수 있는 지 확인하는 조건문
            if(willSendCargo.Count != 0)
            {
                foreach(CargoEvent listCargo in willSendCargo)
                {
                    foreach(ContainerLocation location in sendContainerLoc)
                    {
                        //보낼 컨테이너와 하역장에 내려진 화물이 일치하면 해당 화물 전송
                        if (location.myContainer != null && listCargo.containers[0].code == location.myContainer.GetComponent<TempContainer>().code)
                        {
                            SendCargoEvent(location.myContainer, listCargo);
                            goto escapeSendSequence;
                        }
                        else if (location.myContainer != null && listCargo.containers[0].code != location.myContainer.GetComponent<TempContainer>().code)
                        {
                            Debug.Log("잘못된 컨테이너를 내렸습니다");
                            goto escapeSendSequence;
                        }
                    }
                }
                escapeSendSequence: ;
            }
        }
    }

    //대기하고 있는 배(화물선)가 있는 경우 돌아가는 함수
    private IEnumerator WaitShip()
    {
        while (true)
        {
            //대기하는 배가 있고 현재 배가 없으면 실행
            if (nowShip == null && waitingShips.Count != 0)
            {
                nowShip = waitingShips[0];
                waitingShips.RemoveAt(0);
                GetCargoEvent(nowShip);

                //만약 더 이상 대기하는 배가 없으면 컨테이너 체크 함수 종료
                if (waitingShips == null)
                {
                    StopCoroutine(CheckGetEnd());
                }
            }
            yield return null;
        }
    }

    //컨테이너 다 내렸는지 확인하는 함수
    private IEnumerator CheckGetEnd()
    {
        while (true)
        {
            yield return null;

            int clearCount = 0;

            foreach (ContainerLocation location in getContainerLoc)
            {
                if (location.myContainer == null)
                    clearCount++;
            }

            if (clearCount == getContainerLoc.Count)
            {
                GetScore(100, nowShip.launchTime);
                nowShip = null;
                Debug.Log("Ship is Clear");
            }
        }
    }

    //화물 수령 이벤트 함수
    public void GetCargoEvent(CargoEvent cargoEvent)
    {
        Debug.Log("Cargo is Landed");

        int count = 0;

        //CargoEvent 안의 컨테이너들을 모두 생성해서 배치
        foreach (ContainerInfo containerInfo in cargoEvent.containers)
        {
            //컨테이너 생성 후 배치 장소에 배치
            GameObject nowContainer = CreateContainer(containerInfo);
            nowContainer.transform.position = getContainerLoc[count].transform.position;
            getContainerLoc[count].myContainer = nowContainer;

            nowContainers.Add(nowContainer);

            count++;
        }
    }
    
    //화물 배송 이벤트 함수 (실제 배송)
    public void SendCargoEvent(GameObject sendCargo, CargoEvent nowEvent)
    {
        Debug.Log("배송 완료된 컨테이너 번호 : " + nowEvent.containers[0].code);

        //현재 컨테이너 목록에서 삭제
        nowContainers.Remove(sendCargo);
        willSendCargo.Remove(nowEvent);

        //컨테이너 완전 삭제
        Destroy(sendCargo);

        GetScore(300, nowEvent.launchTime);
    }

    //컨테이너(화물) 생성 함수
    public GameObject CreateContainer(ContainerInfo containerInfo)
    {
        TempContainer tempContainer = Instantiate(basicContainer).GetComponent<TempContainer>();

        //g에 컨테이너 정보 입력 및 컨테이너 외형 생성
        tempContainer.code = containerInfo.code;
        tempContainer.size = containerInfo.size;

        return tempContainer.gameObject;
    }

    //점수 계산 함수(시간이 얼마나 지났냐에 따라서 점수 가감 결정)
    private void GetScore(int originalScore, float originalTime)
    {
        if(mainTime <= originalTime + possibleTime)
        {
            nowScore += originalScore;
        }
        else if(mainTime <= originalTime + possibleTime * 2)
        {
            nowScore += originalScore / 2;
        }
        else
        {
            nowScore -= originalScore / 2;
        }
    }

    //스케줄표 자동 생성
    public void CreateSchadule()
    {        
        int eventCount = 5;
        float nowTimeStamp = 0;
        string nowSchaduleString = "작업 종류 : ";
        List<ContainerInfo> tempNowCargo = new();                              //컨테이너 보내기 이벤트 생성 용 임시 컨테이너 리스트

        //스케줄 간 최소 간격을 유지해서 생성
        for (int i = 0; i < eventCount; i++)
        {
            //발생 타이밍 결정
            nowTimeStamp += minTimeDistance + Random.Range(0.1f, 2.0f);

            nowSchaduleString = "작업 시간 : " + nowTimeStamp.ToString();
            nowSchaduleString += "작업 종류 : ";

            //보내는 이벤트인지 받는 이벤트인지 결정
            if (Random.Range(1, 3) == 1 || tempNowCargo.Count == 0)
            {
                todayEventsType = TodayEventsType.getCargo;
            }
            else
            {
                todayEventsType = TodayEventsType.sendCargo;
            }

            //이벤트 생성
            CargoEvent creatingEvent = new CargoEvent();
            creatingEvent.launchTime = nowTimeStamp;
            creatingEvent.eventType = todayEventsType;
            creatingEvent.containers = new List<ContainerInfo>();

            //만약 화물 받기면 화물 생성
            if(todayEventsType == TodayEventsType.getCargo)
            {
                nowSchaduleString += "화물 수령 \n";
                nowSchaduleString += "받는 컨테이너의 코드 목록 \n";
                
                int cargoCount = 3;

                for(int n = 0; n < cargoCount; n++)
                {
                    ContainerInfo currentInfo = new ContainerInfo();
                    currentInfo.code = Random.Range(1000, 9999).ToString(); //중복되는 코드 생성하지 않는 기능 필요
                    currentInfo.size = new Vector2(2, 1);

                    //임시 리스트에 추가
                    tempNowCargo.Add(currentInfo);

                    creatingEvent.containers.Add(currentInfo);

                    nowSchaduleString += currentInfo.code.ToString() + " / ";
                }
            }
            //만약 화물 보내기면 있는 화물 중에서 선택
            else if(todayEventsType == TodayEventsType.sendCargo)
            {
                nowSchaduleString += "화물 배송 \n";
                nowSchaduleString += "보낼 컨테이너 코드 : ";
                
                ContainerInfo currentInfo = new ContainerInfo();

                //현재 컨테이너 야드에 있는 컨테이너를 배송 대상으로 지정할 때 
                if (nowContainers.Count != 0 && Random.Range(1, 3) == 1)
                {
                    GameObject currentCargo = nowContainers[Random.Range(0, nowContainers.Count)];
                    
                    currentInfo.code = currentCargo.GetComponent<TempContainer>().code;
                    currentInfo.size = currentCargo.GetComponent<TempContainer>().size;
                }
                //오늘 올 컨테이너를 배송 대상으로 지정할 때
                else
                {
                    currentInfo = tempNowCargo[Random.Range(0, tempNowCargo.Count)];
                }

                creatingEvent.containers.Add(currentInfo);

                nowSchaduleString += currentInfo.code.ToString();
            }
            //다 틀렸으면 에러 출력
            else
            {
                Debug.Log("ERROR in CreateSchadule");
            }

            //생성된 이벤트를 이벤트 리스트에 저장
            schaduleList.Add(creatingEvent);

            //유저가 인게임에서 볼 수 있는 형태의 스케줄표 표현
            UnityEngine.UI.Image nowCalender = Instantiate(basicCalendar);
            Text nowSchaduleText = nowCalender.GetComponentInChildren<Text>();

            nowSchaduleText.text = nowSchaduleString;
            nowCalender.transform.SetParent(basicCalendar.transform.parent.GetChild(0), false);
            nowCalender.transform.localPosition = new Vector3(0, -i * nowCalender.rectTransform.sizeDelta.y, 0);
        }
    }
}
