using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public MoveCamera moveCamera;
    public RotateCamera rotateCamera;
    public List<GameObject> cameras = new List<GameObject>();

    private int _current;

    public void Start()
    {
        _current = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            cameras.Add(transform.GetChild(i).gameObject);
        }
    }

    public void ChangeCameraMode()
    {
        //if(moveCamera.enabled == true && rotateCamera.enabled == false)
        //{
        //    moveCamera.enabled = false;
        //    rotateCamera.enabled = true;
        //    //Camera.main.transform.parent = null;
        //    Debug.Log("To Rotate");
        //}
        //else if(moveCamera.enabled == false && rotateCamera.enabled == true)
        //{
        //    moveCamera.enabled = true;
        //    rotateCamera.enabled = false;
        //    //Camera.main.transform.parent = transform;
        //    Debug.Log("To Move");
        //}
        //else
        //{
        //    Debug.LogError("카메라의 두 모드가 동시에 켜졌거나 꺼졌습니다");
        //}
        cameras[(_current + 1) % cameras.Count].SetActive(true);
        cameras[_current].SetActive(false);
        _current = (_current + 1) % cameras.Count;
    }
}
