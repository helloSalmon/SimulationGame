using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerDetail : MonoBehaviour
{
    public enum detailContents
    {
        a,
        b,
        c,
        d
    }

    public int realWeight;
    public int realGarron;
    public detailContents realDetailContent;

    public int declareWeight;
    public int declareGarron;
    public detailContents declareDetailContent;

    public float wrongPercent = 50.0f;
    public bool isWrong;

    public enum AcceptionState { none, pass, nonpass }
    public AcceptionState acceptionState = AcceptionState.none;

    public void Start()
    {
        //실제 상세 수치 결정
        realWeight = Random.Range(30, 100);
        realGarron = Random.Range(30, 100);
        realDetailContent = (detailContents)Random.Range(0, 4);

        //옳은 건지 틀린 건지 결정
        if(wrongPercent <= Random.Range(0,100))
        {
            isWrong = true;

            //틀린 거면 무게나 부피, 코드번호 중 하나를 바꾸기
            if(50 <= Random.Range(0,100))
            {
                declareGarron = realGarron;

                declareWeight = Random.Range(30, 100);
                while (declareWeight != realWeight)
                {
                    declareWeight = Random.Range(30, 100);
                }
            }
            else
            {
                declareWeight = realWeight;

                declareGarron = Random.Range(30, 100);
                while (declareGarron != realGarron)
                {
                    declareGarron = Random.Range(30, 100);
                }
            }

            //일정 확률로 안에 내용물도 바꿈 (단순 누락 실수와 내용물이 다른 것은 처벌 강도를 다르게 할 예정임)
            if (50 <= Random.Range(0, 100))
            {
                declareDetailContent = (detailContents)Random.Range(0, 4);
                while (realDetailContent != declareDetailContent)
                {
                    declareDetailContent = (detailContents)Random.Range(0, 4);
                }
            }
            else
            {
                declareDetailContent = realDetailContent;
            }
        }
        else
        {
            //맞는 거면 전부 맞는 걸로 변경
            declareWeight = realWeight;
            declareGarron = realGarron;
            declareDetailContent = realDetailContent;

            isWrong = false;
        }
    }
}
