using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    //점수 관련 변수들
    public static int currentScore = 0;        //현재 일과의 점수
    public static float permittedEventTime = 100.0f;
    //점수 계산 함수(시간이 얼마나 지났냐에 따라서 점수 가감 결정)
    public static void Calculate(int originalScore, float originalTime, float gameTime)
    {
        if (gameTime <= originalTime + permittedEventTime)
        {
            currentScore += originalScore;
        }
        else
        {
            currentScore -= originalScore / 2;
        }
        Managers.Time.scoreText.text = "Score : " + currentScore;
    }
}
