using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectSO", menuName = "")]
public class PlaceableObjectSO : ScriptableObject
{
    public static Direction GetNextDirection(Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
    }

    public enum Direction
    {
        Right,
        Up,
        Left,
        Down,
    }

    public int GetVoxelPositionList(Vector3Int offset, Direction direction, List<Vector3Int> result)
    {
        result.Clear();

        Vector3Int dir;

        switch (direction)
        {
            case Direction.Right:
                dir = new Vector3Int(1, 1, 1);
                break;
            case Direction.Up:
                dir = new Vector3Int(-1, 1, 1);
                break;
            case Direction.Left:
                dir = new Vector3Int(-1, 1, -1);
                break;
            case Direction.Down:
                dir = new Vector3Int(1, 1, -1);
                break;
            default:
                dir = size;
                break;
        }


        for (int z = 0; z < size.z; z++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector3Int delta = Vector3Int.zero;

                    delta.x = dir.x * x;
                    delta.y = dir.y * y;
                    delta.z = dir.z * z;

                    result.Add(offset + delta);
                }
            }
        }

        return size.x * size.y * size.z;
    }

    public Vector3Int size = Vector3Int.one;
}
