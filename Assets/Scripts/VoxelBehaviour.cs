using UnityEngine;


public class VoxelBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3Int size;

    [SerializeField]
    private float cellSize;

    public Voxel<VoxelObject> voxel { get; private set; }


    private void Awake()
    {
        this.voxel = new Voxel<VoxelObject>(size, () =>
        {
            VoxelObject newObject = new VoxelObject();
            return newObject;
        });
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        Vector3 voxelFloatPosition = (worldPosition - transform.position) / cellSize;
        Vector3Int voxelPosition = new Vector3Int();
        voxelPosition.x = Mathf.FloorToInt(voxelFloatPosition.x);
        voxelPosition.y = Mathf.FloorToInt(voxelFloatPosition.y);
        voxelPosition.z = Mathf.FloorToInt(voxelFloatPosition.z);

        return voxelPosition;
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPosition)
    {
        return transform.position + (Vector3)cellPosition * cellSize + Vector3.one * cellSize / 2;
    }

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
