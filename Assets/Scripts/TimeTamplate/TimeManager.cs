using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    //컨테이너 정보
    public class ContainerInfo
    {
        public int code;
        public Vector3 size;
    }

    //스케줄 클래스
    public class CargoEvent
    {
        public List<ContainerInfo> containers;
        public float launchTime;
        public TodayEventsType eventType;
    }
    
    //스케줄 이벤트 종류
    private enum TodayEventsType
    {
        getCargo,
        sendCargo
    }
    private TodayEventsType todayEventsType;
    
    //시간 관련 변수들
    public float mainTime;
    public float timeSpeed = 1.0f;
    public float minTimeDistance;
    public float maxTimeDistance;

    //스케줄 표
    public List<CargoEvent> schduleList;
    private float minTime = 5.0f;

    //컨테이너 관련 변수들
    public GameObject basicContainer;                              //기본 컨테이너
    public GameObject tempContainer;                              //테스트용 임시 컨테이너 (여기에 화물코드만 있는 임시 컨테이너 스크립트 필요)
    public List<Transform> getContainerLoc;                       //컨테이너가 생성되는 위치
    public List<GameObject> sendContainerLoc;                  //보낼 컨테이너를 저장할 홀더들 (옮겨진 컨테이너의 정보를 확인할 스크립트 필요)
    public List<GameObject> nowContainers;
    public CargoEvent nowShip;
    public List<CargoEvent> waitingShips;

    //UI
    public Text timeText;

    private IEnumerator TimeGoing()
    {
        //배속에 맞게 시간을 돌림
        mainTime += Time.deltaTime * timeSpeed;

        //mainTime을 60진법에 맞게 변환 (게임 시간 내 속도로 변환) 

        //schaduleList의 첫 번째 항목이 나올 시간이 되었는지 체크
        if(schduleList[0].launchTime <= mainTime)
        {
            //시간이 됐으면 스케줄 실행 함수에 옮기기
            CargoEvent nowEventCargos = schduleList[0];

            //실행된 스케줄을 삭제
            schduleList.RemoveAt(0);

            //스케줄 종류를 구분하고 그에 맞는 함수 실행
            switch(nowEventCargos.eventType)
            {
                case TodayEventsType.getCargo:
                    if(nowShip == null)
                    {
                        GetCargoEvent(nowEventCargos);
                    }
                    else
                    {
                        waitingShips.Add(nowEventCargos);
                    }
                    break;

                case TodayEventsType.sendCargo:
                    SendCargoEvent(nowEventCargos.containers[0]); // 이거 CheckCargo로 바꾸어야 함
                    break;
            }
        }
        yield return new WaitForSeconds (0.1f);
    }

    private IEnumerator WaitShip()
    {
        
    }

    //화물 수령 이벤트 함수
    public void GetCargoEvent(CargoEvent cargoEvent)
    {
        
        int count = 0;

        //CargoEvent 안의 컨테이너들을 모두 생성해서 배치
        foreach (ContainerInfo containerInfo in cargoEvent.containers)
        {
            //컨테이너 생성 후 배치 장소에 배치
            GameObject nowContainer = CreateContainer(containerInfo);
            nowContainer.transform.position = getContainerLoc[count].position;

            //현재 컨테이너 목록에 추가
            nowContainers.Add(nowContainer);

            count++;
        }
    }

    //화물 배송 이벤트 함수 (배송할 화물이 왔는지 체크)
    private IEnumerator CheckCargo(GameObject sendCargo)
    {
        yield return new WaitForSeconds(0.1f);

        //만약 배송 홀더에 화물이 도착하면 sendCargo와 일치하는지 확인
        foreach(GameObject g in sendContainerLoc)
        {
            //배송 홀더의 컨테이너가 sendCargo와 일치하면 SendCargoEvent(sendCargo);
        }
    }

    //화물 배송 이벤트 함수 (실제 배송)
    public void SendCargoEvent(GameObject sendCargo)
    {        
        //현재 컨테이너 목록에서 삭제
        nowContainers.Remove(sendCargo);

        //컨테이너 완전 삭제
        Destroy(sendCargo);
    }

    //컨테이너(화물) 생성 함수
    public GameObject CreateContainer(ContainerInfo containerInfo)
    {
        //GameObject g = Instantiate(basicContainer);
        GameObject g = Instantiate(tempContainer);

        //g에 컨테이너 정보 입력 및 컨테이너 외형 생성

        return g;
    }

    //스케줄표 자동 생성
    public void CreateSchadule()
    {        
        int eventCount = 5;
        float nowTimeStamp = 0;
        
        //스케줄 간 최소 간격을 유지해서 생성
        for (int i = 0; i < eventCount; i++)
        {
            //발생 타이밍 결정
            nowTimeStamp += minTimeDistance + 2.0f;
            
            //보내는 건지 받는 건지 결정
            todayEventsType = TodayEventsType.getCargo;

            //이벤트 생성
            CargoEvent creatingEvent = new CargoEvent();
            creatingEvent.launchTime = nowTimeStamp;
            creatingEvent.eventType = todayEventsType;

            //만약 화물 받기면 화물 생성
            if(todayEventsType == TodayEventsType.getCargo)
            {
                int cargoCount = 3;

                for(int i = 0; i < cargoCount; i++)
                {
                    ContainerInfo currentInfo = new ContainerInfo();
                    currentInfo.code = 2000;

                    creatingEvent.containers.Add(currentInfo);
                }
            }
            //만약 화물 보내기면 있는 화물 중에서 선택
            else if(todayEventsType == TodayEventsType.sendCargo)
            {
                GameObject currentCargo = nowContainers[0];
                creatingEvent.containers.Add(currentCargo);               
            }
            //다 틀렸으면 에러 출력
            else
            {
                Debug.Log("ERROR in CreateSchadule");
            }

            //생성된 이벤트를 이벤트 리스트에 저장
            schduleList.Add(creatingEvent);
        }
    }
}
