using UnityEngine;
using UnityEngine.Assertions;
using System;


public class Voxel<TVoxelObject> where TVoxelObject : class
{
    private TVoxelObject[,,] voxelArray;
    private Vector3Int size;


    public Voxel(Vector3Int size, Func<TVoxelObject> create)
    {
        this.size = size;

        this.voxelArray = new TVoxelObject[size.z, size.y, size.x];

        for (int z = 0; z < size.z; z++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    this.voxelArray[z, y, x] = create.Invoke();
                }
            }
        }
    }

    public TVoxelObject GetVoxelObject(Vector3Int position)
    {
        if (position.x < 0 || position.x >= size.x ||
            position.y < 0 || position.y >= size.y ||
            position.z < 0 || position.z >= size.z)
            return default(TVoxelObject);

        return voxelArray[position.z, position.y, position.x];
    }

}
