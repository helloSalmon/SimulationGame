using System.Collections.Generic;
using UnityEngine;
using System;


public class PlaceableObject : MonoBehaviour
{
    public enum Direction
    {
        Right = 0,
        Up = 1,
        Left = 2,
        Down = 3,
    }


    public IReadOnlyList<Vector3Int> occupyPosition => m_occupyPosition;
    private readonly List<Vector3Int> m_occupyPosition = new List<Vector3Int>();

    [field: SerializeField]
    public Vector3Int Size { get; private set; } = Vector3Int.one;
    public Func<bool> CanRemove = () => true;

    public static Direction GetNextDirection(Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
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
                dir = Size;
                break;
        }


        for (int z = 0; z < Size.z; z++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    Vector3Int delta = Vector3Int.zero;

                    delta.x = dir.x * x;
                    delta.y = dir.y * y;
                    delta.z = dir.z * z;

                    result.Add(offset + delta);
                }
            }
        }

        return Size.x * Size.y * Size.z;
    }

    public void Place(IReadOnlyList<Vector3Int> positions)
    {
        m_occupyPosition.Clear();
        m_occupyPosition.AddRange(positions);
    }

    public void Remove()
    {
        m_occupyPosition.Clear();
    }
}
