using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealInfoPanel : MonoBehaviour
{
    public Blocker blocker;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ShowPanel()
    {
        blocker.TurnOn();
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        blocker.TurnOff();
        gameObject.SetActive(false);
    }
}
