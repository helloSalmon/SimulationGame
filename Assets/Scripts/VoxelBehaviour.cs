using UnityEngine;


public class VoxelBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3Int size;

    private Voxel<VoxelObject> voxel;


    private void Awake()
    {
        this.voxel = new Voxel<VoxelObject>(size);
    }
}
