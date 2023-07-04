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

    // todo: 컨테이너 사이즈는 Vector3Int임
    Vector2 IContainerInfo.Size
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }


    private string code;
}
