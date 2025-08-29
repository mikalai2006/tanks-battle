using System;
using UnityEngine;

namespace Mikalai2006.Voxel {
    public static class TestHelpers
    {
        // public static void LoopPositions(Action<int, Vector3> action)
        // {
        //     var i = 0;
        //     for (var y = 0; y < Depth; y++)
        //     {
        //         for (var x = 0; x < SIDE_LENGTH; x++)
        //         {
        //             for (var z = 0; z < SIDE_LENGTH; z++)
        //             {
        //                 action(i++, new Vector3(x, y, z));
        //             }
        //         }
        //     }
        // }

        // Helper function to check if a point is inside a sphere
        public static bool IsInsideSphere(Vector3 point, Vector3 sphereCenter, float sphereRadius)
        {
            return Vector3.Distance(point, sphereCenter) <= sphereRadius;
        }
    }
}