using UnityEngine;
using System.Collections.Generic;


public class VoxelBuildingSystem : MonoBehaviour
{
    private static readonly List<Vector3Int> positionCache = new List<Vector3Int>(32);




    public VoxelBuildingSystem()
    {

    }


    public void PlaceBuilding(PlaceableObjectSO placeableObjectSO, Vector3Int origin, PlaceableObjectSO.Direction direction)
    {
        if (!CanBuild(placeableObjectSO, origin, direction))
            return;

        int positionCount = placeableObjectSO.GetVoxelPositionList(origin, direction, positionCache);

        for (int i = 0; i < positionCount; i++)
        {

        }
    }

    public void RemoveBuilding()
    {

    }

    public bool CanBuild(PlaceableObjectSO placeableObjectSO, Vector3Int origin, PlaceableObjectSO.Direction direction)
    {
        int positionCount = placeableObjectSO.GetVoxelPositionList(origin, direction, positionCache);

        for (int i = 0; i < positionCount; i++)
        {

        }

        positionCache.Clear();

        return true;
    }
}
