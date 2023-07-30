using UnityEngine;
using System;


/// <summary>
/// 배치 가능한 오브젝트 중 정보를 담고 있는 컨테이너 컴포넌트
/// </summary>
public class Container : PlaceableObject, IContainerInfo
{
    public string Code
    {
        get => code;
        set => code = value;
    }

    [SerializeField]
    private int cellSize;

    public Vector3Int Size
    {
        get => cellSize * _size;
    }

    private string code;

    // 현재 자신과 연관되어 있는 화물 이벤트를 나타낸다.
    public CargoEvent cargoEvent;

    public void DeleteEvent()
    {
        if (cargoEvent == null) return;

        if (cargoEvent.type == CargoEventType.Delivery)
        {
            cargoEvent.Unsubscribe();
        }
    }
}
