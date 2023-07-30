using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    int count = 0;

    public void TurnOn()
    {
        count++;
        if (count > 0)
        {
            gameObject.SetActive(true);
        }
    }

    public void TurnOff()
    {
        count--;
        if (count == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
