using UnityEngine;
using System.Collections.Generic;


public class VoxelBuildingSystem : MonoBehaviour
{
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
