using System.Collections.Generic;
using UnityEngine;
using System;


public class PlaceableObject : MonoBehaviour
{
    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }

    public IReadOnlyList<Vector3Int> occupyPosition => m_occupyPosition;
    [SerializeField]
    private List<Vector3Int> m_occupyPosition = new List<Vector3Int>();

    [field: SerializeField]
    protected Vector3Int _size = Vector3Int.one;
    public Func<bool> CanRemove = () => true;

    private static Quaternion[] directions = {
        Quaternion.AngleAxis(0, Vector3.up), // right
        Quaternion.AngleAxis(90, Vector3.up), // up
        Quaternion.AngleAxis(180, Vector3.up), // left
        Quaternion.AngleAxis(270, Vector3.up), // down
    };

    public static Direction GetDirectonFromRotation(Quaternion rotation)
    {
        float minAngle = float.MaxValue;
        int min = 0;

        for (int i = 0; i < 4; i++)
        {
            float angle = Quaternion.Angle(rotation, directions[i]);

            if (angle < minAngle)
            {
                minAngle = angle;
                min = i;
            }
        }

        return (Direction)min;
    }

    /// <summary>
    /// 다음 방향을 구하는 함수, right -> up -> left -> down -> right
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Direction GetNextDirection(Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
    }

    /// <summary>
    /// 배치 했을 때 차지하게 될 복셀 좌표를 반환
    /// </summary>
    /// <param name="offset">시작점</param>
    /// <param name="direction">방향</param>
    /// <param name="result">값을 반환받을 복셀 좌표 값</param>
    /// <returns>차지 중인 복셀의 수</returns>
    public int GetVoxelPositionList(Vector3Int offset, Direction direction, List<Vector3Int> result)
    {
        result.Clear();

        Vector3Int dir, where;

        switch (direction)
        {
            case Direction.Right:
                dir = new Vector3Int(_size.x, 1, _size.z);
                where = Vector3Int.right;
                break;
            case Direction.Up:
                dir = new Vector3Int(_size.z, 1, _size.x);
                where = Vector3Int.forward;
                break;
            case Direction.Left:
                dir = new Vector3Int(-_size.x, 1, _size.z);
                where = Vector3Int.left;
                break;
            case Direction.Down:
                dir = new Vector3Int(-_size.z, 1, -_size.x);
                where = Vector3Int.back;
                break;
            default:
                dir = _size;
                where = Vector3Int.forward;
                break;
        }

        for (int z = 0; z < Mathf.Abs(dir.z); z++)
        {
            for (int y = 0; y < Mathf.Abs(dir.y); y++)
            {
                for (int x = 0; x < Mathf.Abs(dir.x); x++)
                {
                    Vector3Int delta = dir;
                    delta.x /= Mathf.Abs(delta.x);
                    delta.y /= Mathf.Abs(delta.y);
                    delta.z /= Mathf.Abs(delta.z);
                    delta.x *= x;
                    delta.y *= y;
                    delta.z *= z;
                    result.Add(delta);
                }
            }
        }

        for (int i = 0; i < result.Count; ++i)
        {
            result[i] = result[i] - where + offset;
        }

        return _size.x * _size.y * _size.z;
    }

    public void SetDirection(Direction direction)
    {
        transform.rotation = directions[(int)direction];
    }

    /// <summary>
    /// 배치 시 호출되는 함수
    /// </summary>
    /// <param name="positions">실제로 차지중인 복셀 좌표</param>
    public void Place(IReadOnlyList<Vector3Int> positions)
    {
        m_occupyPosition.Clear();
        m_occupyPosition.AddRange(positions);
    }

    /// <summary>
    /// 제거 시 호출되는 함수
    /// </summary>
    public void Remove()
    {
        m_occupyPosition.Clear();
    }
}
