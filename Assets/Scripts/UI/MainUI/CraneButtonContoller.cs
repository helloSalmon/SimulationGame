using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneButtonContoller : MonoBehaviour
{
    private CraneSelectionButton craneSelectionButton;

    

    private void Start()
    {
        craneSelectionButton = gameObject.GetComponent<CraneSelectionButton>();
    }

    //크레인 후크 올리고 내리는 버튼
    public void HookUpAndDown(int upordown)
    {
        craneSelectionButton.myCrane.OnKeyPressed(upordown);
    }

    //크레인 후크 돌리는 버튼
    public void HookRotation()
    {
        if (craneSelectionButton.myCrane.hookDir == Vector3.right)
            craneSelectionButton.myCrane.UpdateSpinning(1);
        else if (craneSelectionButton.myCrane.hookDir == Vector3.forward)
            craneSelectionButton.myCrane.UpdateSpinning(2);
    }
}
