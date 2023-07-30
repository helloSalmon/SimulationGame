using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SearchContainerInfo : MonoBehaviour
{
    private float touchLength;

    public Text containerInfoText;

    private void Start()
    {
        // Input.simulateMouseWithTouches = true;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if(Input.GetMouseButtonDown(0))
        {
            touchLength = 0;
        }

        if(Input.GetMouseButton(0))
        {
            touchLength += Time.deltaTime;

            if(touchLength > 1.0f)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;

                if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, LayerMask.GetMask("Block")))
                {
                    //이후 다른 스크립트 클래스로 바꿔주어야 함
                    if (raycastHit.transform.TryGetComponent(out SettingContainer shownContainer))
                    {
                        Debug.DrawRay(ray.origin, ray.direction, Color.green);
                        containerInfoText.text = shownContainer.regularCode;
                    }                    
                }
            }
        }
    }
}
