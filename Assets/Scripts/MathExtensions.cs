using UnityEngine;


public static class MathExtensions
{
    public static int Manhattan(this Vector3Int v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
    }

    public static int Manhattan(this Vector2Int v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }

    /// <summary>
    /// (x, y, z) -> (x, 0, y)
    /// </summary>
    public static Vector3Int X0Y(this Vector3Int v)
    {
        return new Vector3Int(v.x, 0, v.y);
    }


    /// <summary>
    /// (x, y) -> (x, 0, y)
    /// </summary>
    public static Vector3Int X0Y(this Vector2Int v)
    {
        return new Vector3Int(v.x, 0, v.y);
    }
}
