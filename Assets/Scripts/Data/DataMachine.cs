using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class DataMachine
{
    public float speed;
    public float speedRotateTower;
    public float armour;
    public float hp;
    [Tooltip("Может ли машины стрелять")]
    public bool isShot;
    [Tooltip("Позиция машины")]
    public Vector3 position;
    [Tooltip("Направление передвижения")]
    public Vector3 directionMove;
    [Tooltip("Угол поворота башни")]
    public float angleTower;
    [Tooltip("Фактический угол поворота башни")]
    public float currentAngleTower;
    [Tooltip("Угол поворота базы")]
    public float angleBody;
    [Tooltip("Направление куда смотрит башня со стволом(ами)")]
    public Vector3 directionTower;
    [Tooltip("Время до обнаружения противника")]
    public float timeBeforeAddTarget;
    [Tooltip("Бонусы")]
    public SerializedDictionary<TypeBonus, DataBonus> bonuses;
    // [Tooltip("Значения бонусов")]
    // public SerializedDictionary<TypeBonus, float> bonusesValue;
    [Tooltip("Столы машины")]
    public List<BaseMuzzle> muzzles;

    // [Tooltip("Время от последнего выстрела")]
    // public float timeAfterLastShot;
    // [Tooltip("Дуло, которое сделало последний выстрел")]
    // public BaseMuzzle muzzleLastShot;

    public DataMachine()
    {
        muzzles = new();
        bonuses = new();
        // bonusesValue = new();
    }
}

[Serializable]
public class DataMuzzle
{
    public int index;
    [Tooltip("Количество выстрелов в серии")]
    public float countShotSeria;
    [Tooltip("Время до перезарядки")]
    public float timeBeforeShot;
}


[Serializable]
public class DataBonus
{
    public string id;
    [Tooltip("Время действия")]
    public float time;
    [Tooltip("Значение бонуса (которое добавляется к постоянному значению опред. параметра)")]
    public float value;
}