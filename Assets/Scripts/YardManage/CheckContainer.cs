using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버튼에 달려있는 클래스임
public class CheckContainer : MonoBehaviour
{
    public CraneController selectedCrane;
    public TimeManager timeManager;
    private Schedule schedule;

    public string myCode;

    [SerializeField]
    private GameObject realInfoPanel;
    private Text realWeight;
    private Text realGarron;
    private Text realCode;
    private Text realContent;

    [SerializeField]
    private GameObject declareInfoPanel;
    private Text declareWeight;
    private Text declareGarron;
    private Text declareCode;
    private Text declareContent;

    private void Start()
    {
        realWeight = realInfoPanel.transform.GetChild(2).GetComponent<Text>();
        realGarron = realInfoPanel.transform.GetChild(3).GetComponent<Text>();
        realCode = realInfoPanel.transform.GetChild(4).GetComponent<Text>();
        realContent = realInfoPanel.transform.GetChild(5).GetComponent<Text>();

        declareWeight = declareInfoPanel.transform.GetChild(2).GetComponent<Text>();
        declareGarron = declareInfoPanel.transform.GetChild(3).GetComponent<Text>();
        declareCode = declareInfoPanel.transform.GetChild(4).GetComponent<Text>();
        declareContent = declareInfoPanel.transform.GetChild(5).GetComponent<Text>();

        schedule = timeManager._schedule;
    }

    //확인하기 버튼 누르면 발동됨 - 현재 크레인에 매달려있는 화물 확인
    public void CheckRealDetail()
    {
        //크레인을 선택했고 선택한 크레인의 후크에 컨테이너가 걸렸을 때 발동
        if(selectedCrane != null && selectedCrane.transform.GetChild(3).childCount == 2 && !realInfoPanel.activeSelf)
        {
            realInfoPanel.SetActive(true);

            ContainerDetail selectedContainer = selectedCrane.transform.GetChild(3).GetChild(1).GetComponent<ContainerDetail>();

            realWeight.text = selectedContainer.realWeight.ToString();
            realGarron.text = selectedContainer.realGarron.ToString();
            realCode.text = selectedContainer.gameObject.GetComponent<TempContainer>().Code;
            realContent.text = selectedContainer.realDetailContent.ToString();
        }
        else if(realInfoPanel.activeSelf)
        {
            realInfoPanel.SetActive(false);
        }
    }

    public void CheckDeclareDetail()
    {
        
    }
}
