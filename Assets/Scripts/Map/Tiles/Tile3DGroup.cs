using System.Collections.Generic;
using Mikalai2006.Voxel;
using UnityEngine;

public class Tile3DGroup : MonoBehaviour, IHealthed
{
    [SerializeField] List<Tile3D> tiles;
    public List<Tile3D> Tiles => tiles;
    [SerializeField] protected VoxelMeshRender[] voxelMeshRenders;
    [SerializeField] protected ContainerData containerData;

    void Awake()
    {
        tiles = new List<Tile3D>();
    }

    void RefreshChildrenVoxelMeshRenderer()
    {
        List<VoxelMeshRender> _temp = new List<VoxelMeshRender>();

        foreach (var tile in tiles)
        {
            VoxelMeshRender[] vms = tile.transform.GetComponentsInChildren<VoxelMeshRender>();

            _temp.AddRange(vms);
        }

        voxelMeshRenders = _temp.ToArray();
    }

    public void AddTile(Tile3D tile)
    {
        tiles.Add(tile);

        RefreshChildrenVoxelMeshRenderer();
    }

    public void RemoveTile(Tile3D tile)
    {
        tiles.Remove(tile);

        RefreshChildrenVoxelMeshRenderer();
    }

    public void RefreshHP()
    {
        // анализируем данные о разрушениях и обновляем данные уровня здоровья группы объектов.
        var result = new ContainerData();

        foreach (var vm in voxelMeshRenders)
        {
            if (vm.Containers != null)
            {
                for (int i = 0; i < vm.Containers.Length; i++)
                {
                    result.countVoxelsDestructible += vm.Containers[i].ContainerData.countVoxelsDestructible;
                    result.countVoxels += vm.Containers[i].ContainerData.countVoxels;
                }
            }
        }

        result.levelDestruction = (float)result.countVoxelsDestructible / result.countVoxels;

        containerData = result;
    }
}