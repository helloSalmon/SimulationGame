using UnityEngine;
using UnityEngine.Assertions;


public class Voxel<TVoxelObject>
{
    private TVoxelObject[,,] voxelArray;
    private Vector3Int size;


    public Voxel(Vector3Int size)
    {
        this.size = size;

        this.voxelArray = new TVoxelObject[size.z, size.y, size.x];
    }

    public TVoxelObject GetVoxelObject(Vector3Int position)
    {
        if (position.x < 0 || position.x >= size.x ||
            position.y < 0 || position.y >= size.y ||
            position.z < 0 || position.z >= size.z)
            return default(TVoxelObject);

        return voxelArray[position.z, position.y, position.x];
    }

    public void SetVoxelObject(Vector3Int position, TVoxelObject value)
    {
        if (position.x < 0 || position.x >= size.x ||
            position.y < 0 || position.y >= size.y ||
            position.z < 0 || position.z >= size.z)
            return;

        voxelArray[position.z, position.y, position.x] = value;
    }


}
