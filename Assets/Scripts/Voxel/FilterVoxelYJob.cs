using Mikalai2006.Voxel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct FilterYVoxelJob : IJobFilter
{
    [ReadOnly] public NativeArray<Voxel> arrayVoxels;
    public int y;

    public bool Execute(int index)
    {
        Voxel voxel = arrayVoxels[index];
        bool isNeed = voxel.position.y == y && voxel.type != VoxelType.Destroyed && voxel.type != VoxelType.Air;
        // Return true if the element at 'index' should be kept, false otherwise
        return isNeed;
    }
}