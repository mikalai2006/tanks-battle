using UnityEngine;

[System.Serializable]
public class AreaSearchData
{
    [Tooltip("Находится ли в зоне обнаружения")]
    public bool isInArea;
    [Tooltip("Видим ли объект (нет ли препятствий между объектами)")]
    public bool isVisible;
    [Tooltip("Дистанция до объекта")]
    public float distance;
    [Tooltip("Время прошедшее с момента обнаружения")]
    public float timeView;
    
    // TODO Delete down
    public AreaSearch areaSearch;

}