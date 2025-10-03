using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Mikalai2006.Voxel
{
    
[BurstCompile]
    struct CheckCollisionJob : IJobParallelFor
    {
        public NativeArray<float3> points;
        public float3 _pointCollision;
        public float _radiusExplode;
        public NativeArray<float3> _needCreateElements;
        public NativeArray<float3> needRemoveElements;
        public int maxRadius;

        public void Execute(int index)
        {
            float3 point = points[index];
            if (Helpers.IsInsideSphere(point, _pointCollision, _radiusExplode))
            {
                // list.Add(posx, data.ElementAt(j).Value);
                // // data[posx] = new Voxel()
                // // {
                // //     ID = 0,
                // // };
                // data.Remove(posx);
                needRemoveElements[index] = point;

                if (Helpers.IsInsideSphere(point, _pointCollision, Math.Max(4, Math.Min(_radiusExplode / 2, maxRadius))))
                {
                    _needCreateElements[index] = point;
                }
            }
            else
            {
                _needCreateElements[index] = float3.zero;
                needRemoveElements[index] = float3.zero;
            }
        }
    }
}