using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingContainer : MonoBehaviour
{
    public string regularCode;

    private string colorCode;
    public List<Color> colorList;

    private string companyCode;
    private int companyNumber;
    public List<string> companyCodeList;
    public List<Material> companyPattern;

    // Start is called before the first frame update
    private void Start()
    {
        CreateContainerCode();
    }

    //컨테이너 앞부분 정규코드 생성
    public void CreateContainerCode()
    {
        colorCode = string.Format("{0:D3}", Random.Range(1, 10));

        companyNumber = Random.Range(0, companyCodeList.Count);
        companyCode = companyCodeList[companyNumber];

        CreateContainerLooking();
    }

    private void CreateContainerLooking()
    {
        GetComponent<Renderer>().material.color = colorList[int.Parse(colorCode)];
        //컨테이너 위쪽에 2D Sprite를 넣어서 문양 생성하기
    }
}
