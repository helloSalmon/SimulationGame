using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//컨테이너에 직접 붙이는 스크립트
public class SettingContainer : MonoBehaviour
{
    public string regularCode;
    private string colorCode;
    private string companyCode;

    //컨테이너 앞부분 정규코드 생성
    public void init(string regularCode)
    {
        this.regularCode = regularCode;
        colorCode = regularCode.Substring(5, 3);
        companyCode = regularCode.Substring(9, 3);
        CreateContainerLooking();
    }

    private void CreateContainerLooking()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = Managers.Container.colorList[int.Parse(colorCode)];

        //컨테이너 위쪽에 2D Sprite를 넣어서 문양 생성하기
        GetComponentInChildren<SpriteRenderer>().sprite = Managers.Container.GetCompanySprite(companyCode);
    }

}
