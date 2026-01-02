// using Mikalai2006.Voxel;
// using UnityEngine;

// [CreateAssetMenu(fileName = "SOVoxelDataTile", menuName = "SO/VoxelDataTile")]
// public class SOVoxelDataTile : SOVoxelData
// {
//     public string uid;
//     [Range(1, 100)]
//     public int Weight = 50;
//     public RotationType Rotation;

//     [Space(15)]
//     [Header("Tiledata")]
//     [HideInInspector]public Voxel[] ColorsRight;
//     [HideInInspector] public Voxel[] ColorsForward;
//     [HideInInspector] public Voxel[] ColorsLeft;
//     [HideInInspector] public Voxel[] ColorsBack;
//     [HideInInspector] public Voxel[] ColorsTop;
//     [HideInInspector] public Voxel[] ColorsBottom;
    
//     [Tooltip("Розетки")]
//     public TileSockets tileSockets;
//     [Tooltip("Возможные соседи")]
//     public TileNeghboursList TileNeghboursList;
// }


// [System.Serializable]
// public enum RotationType
// {
//     OnlyRotation,
//     TwoRotations,
//     FourRotations
// }