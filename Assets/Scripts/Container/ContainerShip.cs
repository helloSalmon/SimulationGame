using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerShip : MonoBehaviour
{
    Animator _anim;
    public List<ContainerLocation> containerLocations = new List<ContainerLocation>();

    public ContainerShip init()
    {
        _anim = GetComponent<Animator>();
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            containerLocations.Add(gameObject.transform.GetChild(i).GetComponent<ContainerLocation>());
        }
        return this;
    }


    void Update()
    {
        
    }

    public void EnterPort()
    {
        gameObject.SetActive(true);
        _anim.Play("ShipEnter");
    }

    public void ExitPort()
    {
        _anim.SetTrigger("Exit");
        StartCoroutine(SetActiveFalse());
    }

    IEnumerator SetActiveFalse()
    {
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }
}
