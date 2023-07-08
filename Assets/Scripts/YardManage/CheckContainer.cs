using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버튼에 달려있는 클래스임
public class CheckContainer : MonoBehaviour
{
    public CraneController selectedCrane;

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
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name == "Crane")
                {
                    selectedCrane = hit.collider.gameObject.GetComponent<CraneController>();
                }
            }
        }
    }

    //확인하기 버튼 누르면 발동됨 - 현재 크레인에 매달려있는 화물 확인
    public void CheckRealDetail()
    {
        //크레인을 선택했고 선택한 크레인의 후크에 컨테이너가 걸렸을 때 발동
        if(selectedCrane && selectedCrane.container)
        {
            realInfoPanel.SetActive(true);

            ContainerDetail selectedContainer = selectedCrane.container.GetComponent<ContainerDetail>();

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
        declareInfoPanel.SetActive(true);

        ContainerDetail selectedContainer = Managers.Time.yardContainers.Find(x => x.Code == myCode).gameObject.GetComponent<ContainerDetail>();
        declareWeight.text = "무게 : " + selectedContainer.declareWeight.ToString() + "kg";
        declareGarron.text = "부피 : " + selectedContainer.declareGarron.ToString() + "L";
        declareCode.text = "코드번호 : " + selectedContainer.gameObject.GetComponent<Container>().Code;
        declareContent.text = "내용물 : " + selectedContainer.declareDetailContent.ToString();
    }

    public void ChooseAcceptionState(bool isPass)
    {
        ContainerDetail selectedContainer = selectedCrane.container.GetComponent<ContainerDetail>();

        if (isPass)
        {
            selectedContainer.acceptionState = ContainerDetail.AcceptionState.pass;
        }
        else
        {
            selectedContainer.acceptionState = ContainerDetail.AcceptionState.nonpass;
        }
    }
}
