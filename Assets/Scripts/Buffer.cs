using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 유니티에서 사용하는 Physics 쿼리 함수의 인자로 넘겨 줄 리스트, 배열을 모아놓은 클래스
/// </summary>
public static class Buffer
{
    public static readonly List<Vector3Int> vector3IntBuffer = new List<Vector3Int>(32);
    public static readonly Collider[] colliderBuffer = new Collider[64];
}
