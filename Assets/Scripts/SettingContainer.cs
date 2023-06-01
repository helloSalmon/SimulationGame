using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//컨테이너에 직접 붙이는 스크립트
public class SettingContainer : MonoBehaviour
{
    public string regularCode;

    private string colorCode;
    public List<Color> colorList;

    private string companyCode;
    private int companyNumber;
    public List<string> companyCodeList;
    public List<Sprite> companyPattern;

    // Start is called before the first frame update
    private void Start()
    {
        CreateContainerCode();
    }

    //컨테이너 앞부분 정규코드 생성
    public void CreateContainerCode()
    {
        colorCode = string.Format("{0:D3}", Random.Range(1, colorList.Count));

        companyNumber = Random.Range(0, companyCodeList.Count);
        companyCode = companyCodeList[companyNumber];

        regularCode = colorCode + "-" + companyCode;
        GetComponent<TempContainer>().Code += regularCode;

        CreateContainerLooking();
    }

    private void CreateContainerLooking()
    {
        GetComponent<Renderer>().material.color = colorList[int.Parse(colorCode)];

        //컨테이너 위쪽에 2D Sprite를 넣어서 문양 생성하기
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = companyPattern[companyNumber];
    }
}
