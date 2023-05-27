using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 입력받은 Voxel에 건물을 배치하고 파괴하는 기능을 담당하는 클래스
/// </summary>
public class VoxelBuildingSystem : MonoBehaviour
{
    /// <summary>
    /// 해당 Voxel에 placeableObject를 배치
    /// </summary>
    /// <param name="voxel"></param>
    /// <param name="placeableObject">배치하고자 하는 오브젝트</param>
    /// <param name="origin">오브젝트의 시작점</param>
    /// <param name="direction">복셀 공간에서의 오브젝트의 방향, 기본값은 PlaceableObject.Direction.Right</param>
    public void PlaceBuilding(Voxel<VoxelObject> voxel, PlaceableObject placeableObject, Vector3Int origin, PlaceableObject.Direction direction)
    {
        if (!CanBuild(voxel, placeableObject, origin, direction))
            return;

        int positionCount = placeableObject.GetVoxelPositionList(origin, direction, Buffer.vector3IntBuffer);

        for (int i = 0; i < positionCount; i++)
        {
            VoxelObject element = voxel.GetVoxelObject(Buffer.vector3IntBuffer[i]);
            element.isOccupied = true;
        }

        placeableObject.Place(Buffer.vector3IntBuffer);
        placeableObject.CanRemove = () => CanRemove(placeableObject, voxel);
    }

    /// <summary>
    /// 해당 Voxel에 placeableObject가 배치 가능한 지 확인
    /// </summary>
    /// <param name="voxel"></param>
    /// <param name="placeableObject">배치하고자 하는 오브젝트</param>
    /// <param name="origin">오브젝트의 시작점</param>
    /// <param name="direction">복셀 공간에서의 오브젝트의 방향, 기본값은 PlaceableObject.Direction.Right</param>
    /// <returns></returns>
    public bool CanBuild(Voxel<VoxelObject> voxel, PlaceableObject placeableObject, Vector3Int origin, PlaceableObject.Direction direction)
    {
        int positionCount = placeableObject.GetVoxelPositionList(origin, direction, Buffer.vector3IntBuffer);

        for (int i = 0; i < positionCount; i++)
        {
            VoxelObject voxelObject = voxel.GetVoxelObject(Buffer.vector3IntBuffer[i]);

            // 놓으려는 공간이 차지되어 있으면 배치 불가
            if (voxelObject == null || voxelObject.isOccupied)
                return false;
        }

        // 바닥 체크 필요 없음
        if (origin.y == 0)
            return true;

        for (int i = 0; i < positionCount; i++)
        {
            // 바닥 쪽이 비어있으면 배치 불가
            if (!voxel.GetVoxelObject(Buffer.vector3IntBuffer[i] + Vector3Int.down).isOccupied)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 해당 Voxel에서 placeableObject를 제거
    /// </summary>
    /// <param name="voxel"></param>
    /// <param name="placeableObject">제거하고자 하는 오브젝트</param>
    public void RemoveBuilding(PlaceableObject placeableObject, Voxel<VoxelObject> voxel)
    {
        if (!CanRemove(placeableObject, voxel))
            return;

        foreach (Vector3Int position in placeableObject.occupyPosition)
        {
            VoxelObject voxelObject = voxel.GetVoxelObject(position);
            voxelObject.isOccupied = false;
        }

        placeableObject.Remove();
        placeableObject.CanRemove = () => true;
    }

    /// <summary>
    /// 해당 Voxel에서 placeableObject가 제거 가능한 지 확인
    /// </summary>
    /// <param name="voxel"></param>
    /// <param name="placeableObject">제거하고자 하는 오브젝트</param>
    /// <returns></returns>
    public bool CanRemove(PlaceableObject placeableObject, Voxel<VoxelObject> voxel)
    {
        foreach (Vector3Int position in placeableObject.occupyPosition)
        {
            // hack: 높이가 2 이상인 건물에 대해 검증되지 않았음
            if (voxel.GetVoxelObject(position + Vector3Int.up).isOccupied)
                return false;
        }

        return true;
    }
}
