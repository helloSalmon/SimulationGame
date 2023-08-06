using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버튼에 달려있는 클래스임
public class CheckContainer : MonoBehaviour
{
    public ICraneController selectedCrane;

    public string myCode;

    [SerializeField]
    private RealInfoPanel realInfoPanel;
    private Text realTitle;
    private Text realWeight;
    private Text realGarron;
    private Text realCode;
    private Text realContent;

    [SerializeField]
    private DeclaredInfoPanel declaredInfoPanel;
    private Text declaredTitle;
    private Text declaredWeight;
    private Text declaredGarron;
    private Text declaredCode;
    private Text declaredContent;
    /*
    private void Start()
    {
        Transform realTextList = null;
        for (int i = 0; i < realInfoPanel.transform.childCount; i++)
        {
            if (realInfoPanel.transform.GetChild(i).gameObject.name == "RealTextList")
            {
                realTextList = realInfoPanel.transform.GetChild(i);
                break;
            }
        }
        if (realTextList != null)
        {
            realTitle = realTextList.GetChild(0).GetComponent<Text>();
            realWeight = realTextList.GetChild(1).GetComponent<Text>();
            realGarron = realTextList.GetChild(2).GetComponent<Text>();
            realCode = realTextList.GetChild(3).GetComponent<Text>();
            realContent = realTextList.GetChild(4).GetComponent<Text>();
        }

        Transform declareTextList = null;
        for (int i = 0; i < declaredInfoPanel.transform.childCount; i++)
        {
            if (declaredInfoPanel.transform.GetChild(i).gameObject.name == "DeclaredTextList")
            {
                declareTextList = declaredInfoPanel.transform.GetChild(i);
                break;
            }
        }
        if (declareTextList != null)
        {
            declaredTitle = declareTextList.GetChild(0).GetComponent<Text>();
            declaredWeight = declareTextList.GetChild(1).GetComponent<Text>();
            declaredGarron = declareTextList.GetChild(2).GetComponent<Text>();
            declaredCode = declareTextList.GetChild(3).GetComponent<Text>();
            declaredContent = declareTextList.GetChild(4).GetComponent<Text>();
        }
    }

    //확인하기 버튼 누르면 발동됨 - 현재 크레인에 매달려있는 화물 확인
    public void CheckRealDetail()
    {
        //크레인을 선택했고 선택한 크레인의 후크에 컨테이너가 걸렸을 때 발동
        if(selectedCrane != null && selectedCrane.Container != null)
        {
            realInfoPanel.ShowPanel();

            ContainerDetail selectedContainer = selectedCrane.Container.GetComponent<ContainerDetail>();

            realWeight.text = "무게 : " + selectedContainer.realWeight.ToString() + "kg";
            realGarron.text = "부피 : " + selectedContainer.realGarron.ToString() + "L";
            realCode.text = "코드번호 : " + selectedContainer.gameObject.GetComponent<Container>().Code;
            realContent.text = "내용물 : " + selectedContainer.realDetailContent.ToString();
        }
        else
        {
            realWeight.text = "무게 : 알 수 없음";
            realGarron.text = "부피 : 알 수 없음";
            realCode.text = "코드번호 : 알 수 없음";
            realContent.text = "내용물 : 알 수 없음";
        }
    }

    public void CheckDeclareDetail()
    {
        declaredInfoPanel.ShowPanel();

        ContainerDetail selectedContainer = Managers.Time.yardContainers.Find(x => x.Code == myCode).gameObject.GetComponent<ContainerDetail>();
        declaredWeight.text = "무게 : " + selectedContainer.declareWeight.ToString() + "kg";
        declaredGarron.text = "부피 : " + selectedContainer.declareGarron.ToString() + "L";
        declaredCode.text = "코드번호 : " + selectedContainer.gameObject.GetComponent<Container>().Code;
        declaredContent.text = "내용물 : " + selectedContainer.declareDetailContent.ToString();
    }

    public void ChooseAcceptionState(bool isPass)
    {
        ContainerDetail selectedContainer = selectedCrane.Container.GetComponent<ContainerDetail>();

        if (isPass)
        {
            selectedContainer.acceptionState = ContainerDetail.AcceptionState.pass;
        }
        else
        {
            selectedContainer.acceptionState = ContainerDetail.AcceptionState.nonpass;
        }
    }*/
}
