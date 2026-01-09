public interface IHealthed
{
    /// <summary>
    /// Проводит опрос всех дочерних объектов Container на уровень разрушения.
    /// </summary>
    void RefreshHP();
};