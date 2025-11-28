using UnityEngine;

[System.Serializable]
public class AreaSearchData
{
    [Tooltip("Видим ли объект (находится ли в зоне обнаружения)")]
    public bool isVisible;
    [Tooltip("Дистанция до объекта")]
    public float distance;
    [Tooltip("Время прошедшее с момента обнаружения")]
    public float timeView;

}