using System.Collections.Generic;
using UnityEngine;


namespace Mikalai2006.Voxel
{
    public static class HelperVoxel
    {
        #region Static Variables
        public static readonly Vector3[] voxelVertices = new Vector3[8]
        {
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
        };

        public static readonly Vector3[] voxelFaceChecks = new Vector3[6]
        {
            new Vector3(0,0,-1),//back
            new Vector3(0,0,1),//front
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
        };

        // static readonly int[,] voxelVertexIndex = new int[6, 4]
        // {
        //     {0,1,2,3},
        //     {4,5,6,7},
        //     {4,0,6,2},
        //     {5,1,7,3},
        //     {0,1,4,5},
        //     {2,3,6,7},
        // };
        public static readonly int[] voxelVertexIndex = new int[24]
        {
            0,1,2,3,
            4,5,6,7,
            4,0,6,2,
            5,1,7,3,
            0,1,4,5,
            2,3,6,7,
        };

        public static readonly Vector2[] voxelUVs = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1f/256f,0),
            new Vector2(1f/256f,1)
        };

        public static readonly int[] voxelTris = new int[36]
        {
            0,2,3,0,3,1,
            0,1,2,1,3,2,
            0,2,3,0,3,1,
            0,1,2,1,3,2,
            0,1,2,1,3,2,
            0,2,3,0,3,1,
        };

        public static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                    return false;
            }
            return true;
        }

        public static bool AreArraysEqual<T>(T[] list1, T[] list2)
        {
            if (list1.Length != list2.Length)
                return false;

            for (int i = 0; i < list1.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                    return false;
            }
            return true;
        }

        public static bool AreColorEqual(Voxel[] list1, Voxel[] list2)
        {
            if (list1.Length != list2.Length)
                return false;
            // Debug.Log($"{GetArrayHashCode(list1) == GetArrayHashCode(list2)}, {GetArrayHashCode(list1)}, {GetArrayHashCode(list2)}");

            for (int i = 0; i < list1.Length; i++)
            {
                if (
                    list1[i].color.r != list2[i].color.r
                    || list1[i].color.g != list2[i].color.g
                    || list1[i].color.b != list2[i].color.b
                    || list1[i].color.a != list2[i].color.a)
                    return false;
            }

            return true;
        }
        
        // Метод для вычисления хеш-кода на основе содержимого массива
        public static int GetArrayHashCode<T>(T[] array)
        {
            if (array == null)
            {
                return 0;
            }

            // Для примера используем простой метод: суммирование хеш-кодов элементов
            int hash = 17; // Начальное значение
            foreach (var element in array)
            {
                hash = hash * 31 + (element?.GetHashCode() ?? 0);
            }
            return hash;
        }
        #endregion
    }
}