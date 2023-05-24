using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CargoEventType
{
    Shipping,        //컨테이너 수령  
    Delievering      //컨테이너 배송
}

public class ContainerCode
{
    static HashSet<string> _codeSet = new HashSet<string>();
    public static string Make()
    {
        string code = Random.Range(1000, 9999).ToString();
        while (_codeSet.Contains(code))
            code = Random.Range(1000, 9999).ToString();
        _codeSet.Add(code);
        return code;
    }
    public static void Remove(string code)
    {
        _codeSet.Remove(code);
    }
}

public class CargoEvent
{
    public List<ContainerInfo> containers;      //스케줄에 관련된 컨테이너 목록
    public float startTime;                        //스케줄이 오는 시간
    public CargoEventType type;         //스케줄 이벤트 종류
    private int cargoCount;

    public CargoEvent(CargoEventType cargoType, float currentTime, ref string scheduleString, List<GameObject> loadedContainersInYard, List<ContainerInfo> shippingContainer)
    {
        scheduleString = "작업 시간 : " + currentTime.ToString() + "\n";

        startTime = currentTime;
        type = cargoType;
        containers = new List<ContainerInfo>();

        scheduleString += "작업 종류 : ";
        //만약 화물 받기면 화물 생성
        if (type == CargoEventType.Shipping)
        {
            scheduleString += "화물 수령 \n";
            scheduleString += "받는 컨테이너의 코드 목록 \n";
            cargoCount = 3;

            for (int n = 0; n < cargoCount; n++)
            {
                ContainerInfo currentInfo = new ContainerInfo();
                currentInfo.Code = ContainerCode.Make(); //중복되는 코드 생성하지 않는 기능 필요
                currentInfo.Size = new Vector2(2, 1);

                //임시 리스트에 추가
                shippingContainer.Add(currentInfo);

                containers.Add(currentInfo);

                scheduleString += currentInfo.Code.ToString() + " / ";
            }
        }
        //만약 화물 보내기면 있는 화물 중에서 선택
        else if (type == CargoEventType.Delievering)
        {
            scheduleString += "화물 배송 \n";
            scheduleString += "보낼 컨테이너 코드 : ";

            ContainerInfo currentInfo = new ContainerInfo();

            //현재 컨테이너 야드에 있는 컨테이너를 배송 대상으로 지정할 때 
            if (loadedContainersInYard.Count != 0 && Random.Range(1, 3) == 1)
            {
                GameObject container = loadedContainersInYard[Random.Range(0, loadedContainersInYard.Count)];

                currentInfo.Code = container.GetComponent<TempContainer>().Code;
                currentInfo.Size = container.GetComponent<TempContainer>().Size;
            }
            //오늘 올 컨테이너를 배송 대상으로 지정할 때
            else
            {
                currentInfo = shippingContainer[Random.Range(0, shippingContainer.Count)];
            }

            containers.Add(currentInfo);

            scheduleString += currentInfo.Code.ToString();
        }
    }
}
