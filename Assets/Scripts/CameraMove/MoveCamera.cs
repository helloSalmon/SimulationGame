using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Vector3 hitPos;
    private Vector3 nowPos;
    private Vector3 prePos;
    private Vector3 camPos;

    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            hitPos = Input.mousePosition;
            camPos = transform.position;

            prePos = nowPos = Input.mousePosition;
        }
        if(Input.GetMouseButton(0))
        {
            prePos = nowPos;
            nowPos = Input.mousePosition;

            // Debug.Log(prePos);
            // Debug.Log(nowPos);
            // Debug.Log(prePos - nowPos);

            // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
            // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
            //nowPos.z = hitPos.z = camPos.y;

            // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
            Vector3 preDirection = (nowPos - prePos) * 0.5f;
            //Vector3 preDirection = Camera.main.ScreenToWorldPoint(nowPos) - Camera.main.ScreenToWorldPoint(hitPos);
            Vector2 direction = new Vector2(preDirection.x, preDirection.y);
            //direction *= 0.2f;

            // Invert direction to that terrain appears to move with the mouse.
            //direction = direction * -1;

            //transform.position = new Vector3(camPos.x - direction.x, transform.position.y, camPos.z - direction.y);
            transform.Translate(new Vector3(prePos.x - nowPos.x, 0, prePos.y - nowPos.y) * Time.deltaTime * 5.0f, Space.World);
        }
    }
}
