using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour
{
    private Vector3 hitPos;
    private Vector3 nowPos;
    private Vector3 prePos;
    private Vector3 camPos;

    public Quaternion original;
    bool active = false;

    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
        original = transform.rotation;
    }

    Vector3 GetMoveVector(Transform tf, float x, float y)
    {
        Quaternion current = Quaternion.Euler(original.eulerAngles.x, tf.rotation.eulerAngles.y, original.eulerAngles.z);
        Quaternion diff = current * Quaternion.Inverse(original);
        Vector3 dir = new Vector3(x, 0, y);
        dir = diff * dir;
        return dir;
    }

    float Sigmoid(float x)
    {
        return 50 / (1 + Mathf.Exp(5 - x));
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() && active == false)
            return;

        if (active == false && Input.GetMouseButtonDown(1))
        {
            active = true;
            hitPos = Input.mousePosition;
            camPos = transform.position;

            prePos = nowPos = Input.mousePosition;
        }
        if(active == true && Input.GetMouseButton(1))
        {
            // prePos = nowPos;
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
            // Vector2 direction = new Vector2(preDirection.x, preDirection.y);
            //direction *= 0.2f;

            // Invert direction to that terrain appears to move with the mouse.
            //direction = direction * -1;

            Vector3 dir = GetMoveVector(Camera.main.transform, prePos.x - nowPos.x, prePos.y - nowPos.y);
            Vector3 direction = dir.normalized;
            float magnitude = dir.magnitude;
            magnitude = Mathf.Clamp(magnitude, 0.0f, 50.0f);
            magnitude = Sigmoid(magnitude);
            //Vector3 dir = new Vector3(prePos.x - nowPos.x, 0, prePos.y - nowPos.y);

            //transform.position = new Vector3(camPos.x - direction.x, transform.position.y, camPos.z - direction.y);
            transform.Translate(-direction * magnitude * Time.deltaTime, Space.World);
        }
        if (Input.GetMouseButtonUp(1))
        {
            active = false;
        }
    }
}
