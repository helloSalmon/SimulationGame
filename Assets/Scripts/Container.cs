using UnityEngine;
using System;


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
