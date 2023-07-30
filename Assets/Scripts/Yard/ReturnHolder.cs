using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHolder : MonoBehaviour
{
    private ContainerLocation containerLocation;

    // Start is called before the first frame update
    void Start()
    {
        containerLocation = GetComponent<ContainerLocation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(containerLocation.myContainer != null)
        {
            containerLocation.myContainer.GetComponent<CheckWorkRight>().CheckWorkCorrectly(2);
            containerLocation.myContainer.GetComponent<Container>().DeleteEvent();
            Managers.Container.RemoveContainer(containerLocation.myContainer.GetComponent<Container>());
        }
    }
}
