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

    //컨테이너 수령/배송 이벤트의 컨테이너 갯수
    public class CargoEvent
    {
        public List<ContainerInfo> containers;
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

    //스케줄 표
    private List<float> schaduleTimeList;
    private List<TodayEventsType> schaduleTypeList;
    public List<CargoEvent> schduleContainerList;
    private float minTime = 5.0f;

    //컨테이너 관련 변수들
    public GameObject basicContainer;                              //기본 컨테이너
    public GameObject tempContainer;                              //테스트용 임시 컨테이너 (여기에 화물코드만 있는 임시 컨테이너 스크립트 필요)
    public List<Transform> getContainerLoc;                       //컨테이너가 생성되는 위치
    public List<GameObject> sendContainerLoc;                  //보낼 컨테이너를 저장할 홀더들 (옮겨진 컨테이너의 정보를 확인할 스크립트 필요)
    public List<GameObject> nowContainers;

    //UI
    public Text timeText;

    private void Update()
    {
        //배속에 맞게 시간을 돌림
        mainTime += Time.deltaTime * timeSpeed;

        //mainTime을 60진법에 맞게 변환 (게임 시간 내 속도로 변환) 

        //schaduleList의 첫 번째 항목이 나올 시간이 되었는지 체크
        if(schaduleTimeList[0] <= mainTime)
        {
            //시간이 됐으면 스케줄 실행 함수에 옮기기
            TodayEventsType nowEventType = schaduleTypeList[0];
            CargoEvent nowEventCargos = schduleContainerList[0];

            //실행된 스케줄을 삭제
            schaduleTimeList.RemoveAt(0);
            schaduleTypeList.RemoveAt(0);
            schduleContainerList.RemoveAt(0);

            //스케줄 종류를 구분하고 그에 맞는 함수 실행
            switch(nowEventType)
            {
                case TodayEventsType.getCargo:
                    GetCargoEvent(nowEventCargos);
                    break;

                case TodayEventsType.sendCargo:
                    GameObject sendCargo = new GameObject();

                    //받은 정보(화물코드)와 일치하는 컨테이너 찾기
                    foreach(GameObject g in nowContainers)
                    {
                        //g 안에 있는 코드와 nowEventCargos의 코드가 일치하면 sendCargo = g;
                    }
                    SendCargoEvent(sendCargo);
                    break;
            }
        }
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
        //스케줄 간 최소 간격을 유지해서 생성

        //스케줄 생성 시 CargoEvent에서 샤용할 컨테이너 목록을 생성해서 리스트에 저장(ContainerInfo 형태로)
        //
    }
}
