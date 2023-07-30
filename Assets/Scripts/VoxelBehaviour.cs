using UnityEngine;


public class VoxelBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3Int size;

    [SerializeField]
    private int cellSize;

    public Voxel<VoxelObject> voxel { get; private set; }


    private void Awake()
    {
        this.voxel = new Voxel<VoxelObject>(size, () =>
        {
            VoxelObject newObject = new VoxelObject();
            return newObject;
        });
    }

    /// <summary>
    /// World 좌표로부터 Voxel 내의 Cell 좌표로 변환
    /// </summary>
    /// <param name="worldPosition">목표 World 좌표</param>
    /// <returns>World 좌표에 대응되는 Cell 좌표</returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        Vector3 voxelFloatPosition = (worldPosition - transform.position) / cellSize;
        Vector3Int voxelPosition = new Vector3Int();
        voxelPosition.x = Mathf.FloorToInt(voxelFloatPosition.x);
        voxelPosition.y = Mathf.FloorToInt(voxelFloatPosition.y);
        voxelPosition.z = Mathf.FloorToInt(voxelFloatPosition.z);

        return voxelPosition;
    }

    /// <summary>
    /// 해당 Cell의 좌표를 World 좌표로 변환
    /// </summary>
    /// <param name="cellPosition">목표 Cell 좌표</param>
    /// <returns>해당 Cell의 중심 위치</returns>
    public Vector3 GetCellCenterWorld(Vector3Int cellPosition)
    {
        return transform.position + (Vector3)cellPosition * cellSize + Vector3.one * cellSize / 2;
    }

    /// <summary>
    /// 배치 가능한 오브젝트가 배치되었을 때의 world 좌표계 기준 중심 위치를 계산, VoxelBuildingSystem에 의해 배치가 완료된 뒤 호출되어야 함
    /// </summary>
    /// <param name="placeableObject">배치된 오브젝트</param>
    /// <returns>world 좌표계 기준 중심 위치</returns>
    public Vector3 GetPlaceableObjectCenterWorld(PlaceableObject placeableObject)
    {
        Vector3 result = Vector3.zero;

        foreach (Vector3Int position in placeableObject.occupyPosition)
        {
            result += GetCellCenterWorld(position);
        }

        // 모든 위치값의 평균은 중심 위치임
        result /= placeableObject.occupyPosition.Count;

        return result;
    }


    private void OnDrawGizmosSelected()
    {
        // x축 격자 그리기
        for (int z = 0; z <= size.z; z++)
        {
            for (int y = 0; y <= size.y; y++)
            {
                Vector3 origin = transform.position + (Vector3.up * y + Vector3.forward * z) * cellSize;
                Gizmos.DrawLine(origin, origin + Vector3.right * cellSize * size.x);
            }
        }

        // z축 격자 그리기
        for (int y = 0; y <= size.y; y++)
        {
            for (int x = 0; x <= size.x; x++)
            {
                Vector3 origin = transform.position + (Vector3.up * y + Vector3.right * x) * cellSize;
                Gizmos.DrawLine(origin, origin + Vector3.forward * cellSize * size.z);
            }
        }

        // y축 격자 그리기
        for (int z = 0; z <= size.z; z++)
        {
            for (int x = 0; x <= size.x; x++)
            {
                Vector3 origin = transform.position + (Vector3.forward * z + Vector3.right * x) * cellSize;
                Gizmos.DrawLine(origin, origin + Vector3.up * cellSize * size.y);
            }
        }
    }
}
