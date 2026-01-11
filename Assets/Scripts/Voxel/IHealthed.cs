using System.Collections.Generic;

public interface IHealthed
{
    /// <summary>
    /// Проводит опрос всех дочерних объектов Container на уровень разрушения.
    /// </summary>
    void RefreshHP();
    void OnSaveDestroyVoxels(List<RemoveVoxel> voxels, DataDetail dataDetail);
};