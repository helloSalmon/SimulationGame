using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컨테이너에 붙이는 함수로 해당 컨테이너가 제대로 된 위치로 이동했는지 확인하는 역할을 함
/// </summary>
public class CheckWorkRight : MonoBehaviour
{
    private ContainerDetail containerDetail;

    private void Start()
    {
        containerDetail = GetComponent<ContainerDetail>();
    }

    /// <summary>
    /// 해당 컨테이너가 제대로 된 작업이 수행되었는지 확인하는 함수
    /// TODO : DeliveryEvent 클래스에 연결시켜야 함
    /// </summary>
    /// <param name="placeCode">1 = sendholder / 2 = returnHolder</param>
    public void CheckWorkCorrectly(int placeCode)
    {
        switch(placeCode)
        {
            case 1:
                if (containerDetail.acceptionState != ContainerDetail.AcceptionState.pass)
                {
                    Debug.Log("통과되지 않은 물품을 보내버렸습니다.");
                    Score.currentScore -= 100;
                }
                else if (containerDetail.isWrong)
                {
                    Debug.Log("올바르지 않은 물품을 보내버렸습니다.");
                    Score.currentScore -= 100;
                }
                else
                {
                    Debug.Log("제대로 보냈습니다.");
                    Score.currentScore += 100;
                }
                break;

            case 2:
                if (containerDetail.acceptionState != ContainerDetail.AcceptionState.nonpass)
                {
                    Debug.Log("압수처리되지 않은 물품을 압수했습니다.");
                    Score.currentScore -= 100;
                }
                else if (!containerDetail.isWrong)
                {
                    Debug.Log("올바른 물건을 압수했습니다.");
                    Score.currentScore -= 100;
                }
                else
                {
                    Debug.Log("제대로 압수했습니다.");
                    Score.currentScore += 100;
                }
                break;
        }
    }
}
