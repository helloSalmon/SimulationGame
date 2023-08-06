using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIRefresher : MonoBehaviour
{
    public GameObject craneMother;
    private List<GameObject> cranes = new List<GameObject>();
    public GameObject craneSelectTab;
    public GameObject basicCraneSelect;

    public static NewCraneController selectedCrane;

    public GameObject cameraScreen;
    public GameObject paperScreen;
    public GameObject schaduleScreen;

    // Start is called before the first frame update
    void Start()
    {
        //크레인 목록 자체 등록 및 크레인 선택 탭 재설정
        for (int i = 0; i < craneMother.transform.childCount; i++)
        {
            cranes.Add(craneMother.transform.GetChild(i).gameObject);
        }
        RefreshCraneSelectTab();

        paperScreen.SetActive(true);
        paperScreen.SetActive(false);

        schaduleScreen.SetActive(true);
        schaduleScreen.SetActive(false);
    }

    //크레인 선택 탭 초기화 및 재설정
    public void RefreshCraneSelectTab()
    {
        //기존 크레인 선택 버튼 제거
        for (int i = craneSelectTab.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(craneSelectTab.transform.GetChild(i).gameObject);
        }

        //새 크레인 선택 버튼 생성
        for(int i = 0; i < cranes.Count; i++)
        {
            GameObject g = Instantiate(basicCraneSelect, craneSelectTab.transform);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -328 + 240 * i);
            g.GetComponent<CraneSelectionButton>().myCrane = cranes[i].GetComponent<NewCraneController>();
        }
    }
}
