using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public MoveCamera moveCamera;
    public RotateCamera rotateCamera;

    public void ChangeCameraMode()
    {
        if(moveCamera.enabled == true && rotateCamera.enabled == false)
        {
            moveCamera.enabled = false;
            rotateCamera.enabled = true;
            //Camera.main.transform.parent = null;
            Debug.Log("To Rotate");
        }
        else if(moveCamera.enabled == false && rotateCamera.enabled == true)
        {
            moveCamera.enabled = true;
            rotateCamera.enabled = false;
            //Camera.main.transform.parent = transform;
            Debug.Log("To Move");
        }
        else
        {
            Debug.LogError("카메라의 두 모드가 동시에 켜졌거나 꺼졌습니다");
        }
    }
}
