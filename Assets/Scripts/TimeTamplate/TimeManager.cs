using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
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

public class ShipInfo
{
    public CargoEvent currentShip;                                   //현재 컨테이너를 내리고 있는 화물선 (현재 진행 중인 컨테이너 수령 이벤트)
    public List<CargoEvent> waitingShips;                            //뒤에서 대기 중인 화물선 (대기 중인 컨테이너 수령 이벤트)

    public ShipInfo()
    {
        currentShip = null;
        waitingShips = new List<CargoEvent>();
    }
}

public class TimeManager : MonoBehaviour
{
    //시간 관련 변수들
    public float gameTime;                      //현재 일과 중 흘러가는 시간
    public float maxTime;                       //현재 일과의 종료 시간
    public float timeSpeed = 1.0f;            //시간 배속
    public float timer = 1.0f;

    //컨테이너 관련 변수들
    public GameObject basicContainer;                           //기본 컨테이너
    public List<ContainerLocation> containerSpawnLocations;         //컨테이너가 생성되는 위치
    public List<ContainerLocation> containerHolderLocations;       //보낼 컨테이너를 저장할 홀더들 (옮겨진 컨테이너의 정보를 확인할 스크립트 필요)
    public Schedule scheduler;

    public Image basicCalendar;      //스케줄 표 생성에 사용되는 기본 스케줄 표 양식
    public Text timeText;                                   //현재 시각을 표시하는 텍스트
    public Image scheduleCalendar;             //유저가 보는 스케줄 표
    public Text scoreText;

    private void Start()
    {
        scheduler = new Schedule(basicCalendar);
        scheduler.CreateScheduleList(1, 130, 3);
        scheduler.MakeScheduleString();

        Score.currentScore = 0;
        scoreText.text = "Score : 0";

        GameObject go = GameObject.Find("SendLocations");

        for (int i = 0; i < go.transform.childCount; ++i)
        {
            containerHolderLocations.Add(go.transform.GetChild(i).GetComponent<ContainerLocation>());
        }

        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate()
    {
        while (true)
        {
            //배속에 맞게 시간을 돌림
            gameTime += Time.deltaTime * timeSpeed;
            timer += Time.deltaTime;

            //mainTime을 60진법에 맞게 변환 (게임 시간 내 속도로 변환)
            timeText.text = "현재 시각 : \n" + gameTime.ToString();

            scheduler.cargoEventHandler.UpdateTime(gameTime);
            if (timer > 0.5f)
            {
                scheduler.MakeScheduleString();
                timer = 0.0f;
            }

            if (scheduler.CheckEndConditions(gameTime))
                break;

            yield return null;
        }

        scheduler.MakeScheduleString();
        scheduler.ReflectRestScore(gameTime);
        Debug.Log("일과종료");
        SceneManager.LoadScene("GameEndScene");
    }

    public float? GetGameEndTime()
    {
        if (scheduler == null)
            return null;

        return scheduler.gameEndTime;
    }
}
