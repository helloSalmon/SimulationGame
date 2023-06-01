using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCamera : MonoBehaviour
{
    Vector3 FirstPoint;
    Vector3 SecondPoint;
    public float xAngle = 0f;
    public float yAngle = 55f;
    float xAngleTemp;
    float yAngleTemp;

    float rotateSpeed = 0.7f;

    private void Start()
    {
        Input.simulateMouseWithTouches = true;
    }

    private void Update()
    {
        // 마우스가 눌림
        if (Input.GetMouseButtonDown(0))
        {
            FirstPoint = Input.mousePosition;
            xAngleTemp = xAngle;
            yAngleTemp = yAngle;
        }

        if (Input.GetMouseButton(0))
        {
            SecondPoint = Input.mousePosition;
            xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 * rotateSpeed / Screen.width;
            yAngle = yAngleTemp - (SecondPoint.y - FirstPoint.y) * 90 * 3f * rotateSpeed / Screen.height; // Y값 변화가 좀 느려서 3배 곱해줌.            

            transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
        }
    }
}
