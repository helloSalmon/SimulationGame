using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNowTime : MonoBehaviour
{
    public Text timeText;

    public int startHour = 6;
    public int finishHour = 20;
    private float fullMinute;

    private int nowHour;
    private int nowMinute;

    private void Start()
    {
        fullMinute = (finishHour - startHour) * 60;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateTime();

        timeText.text = nowHour.ToString() + " : " + nowMinute.ToString();
    }

    private void CalculateTime()
    {
        float nowTotalMinute = Managers.Time.gameTime * fullMinute / Managers.Time.maxTime;

        nowHour = (int)System.Math.Truncate(nowTotalMinute / 60) + startHour;
        nowMinute = (int)nowTotalMinute % 60;
    }
}
