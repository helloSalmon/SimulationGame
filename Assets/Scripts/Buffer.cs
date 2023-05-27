using System.Collections.Generic;
using UnityEngine;


public static class Buffer
{
    public static readonly List<Vector3Int> vector3IntBuffer = new List<Vector3Int>(32);
    public static readonly Collider[] colliderBuffer = new Collider[64];
}
