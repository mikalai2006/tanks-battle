using Mikalai2006.Voxel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct FilterYJob : IJobFilter
{
    [ReadOnly] public NativeArray<Voxel> dataToFilter; // The array you want to filter based on
    public int y; // Your filtering condition

    public bool Execute(int index)
    {
        Voxel voxel = dataToFilter[index];
        // Return true if the element at 'index' should be kept, false otherwise
        return voxel.position.y == y && voxel.type != VoxelType.Destroyed && voxel.type != VoxelType.Air;
    }
}