using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneSelectionButton : MonoBehaviour
{
    [HideInInspector]
    public NewCraneController myCrane;
    private CraneButtonContoller myButtonContoller;

    public GameObject craneControllerTab;

    private void Start()
    {
        myButtonContoller = gameObject.GetComponent<CraneButtonContoller>();
        craneControllerTab.SetActive(false);
    }

    private void Update()
    {
        if(myCrane != null)
            craneControllerTab.SetActive(myCrane.isSelected);
    }

    public void SelectMyCrane()
    {
        myCrane.isSelected ^= true;

        if (myCrane.isSelected)
        {
            if (MainUIRefresher.selectedCrane)
            {
                MainUIRefresher.selectedCrane.OnDeselected();
                MainUIRefresher.selectedCrane.isSelected = false;
            }

            myCrane.OnSelected();
            MainUIRefresher.selectedCrane = myCrane;
        }
        else
        {
            myCrane.OnDeselected();
            MainUIRefresher.selectedCrane.isSelected = false;
            MainUIRefresher.selectedCrane = null;
        }
    }
}
