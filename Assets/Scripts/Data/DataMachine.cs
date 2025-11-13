using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class DataMachine
{
    public float speed;
    public float armour;
    public float hp;
    [Tooltip("Позиция машины")]
    public Vector3 position;
    [Tooltip("Направление передвижения")]
    public Vector3 directionMove;
    [Tooltip("Угол поворота базы")]
    public float angleBody;
    [Tooltip("Угол поворота базы текущий")]
    public float currentAngleBody;
    [Tooltip("Время до обнаружения противника")]
    public float timeBeforeAddTarget;
    [Tooltip("Бонусы")]
    public SerializedDictionary<TypeBonus, DataBonus> bonuses;
    // [Tooltip("Значения бонусов")]
    // public SerializedDictionary<TypeBonus, float> bonusesValue;
    public List<BaseTower> towers;
    [Tooltip("Уровень разрушения")]
    public float levelDestruction;

    // [Tooltip("Время от последнего выстрела")]
    // public float timeAfterLastShot;
    // [Tooltip("Дуло, которое сделало последний выстрел")]
    // public BaseMuzzle muzzleLastShot;

    public DataMachine()
    {
        bonuses = new();
        towers = new();
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
    [Tooltip("Уровень разрушения")]
    public float levelDestruction;
}

[Serializable]
public class DataBody
{
    public int index;
    [Tooltip("Уровень разрушения")]
    public float levelDestruction;
}

[Serializable]
public class DataCaterpillar
{
    public int index;
    [Tooltip("Уровень разрушения")]
    public float levelDestruction;
}

[Serializable]
public class DataTower
{
    public float speedRotateTower;
    [Tooltip("Направление куда смотрит башня со стволом(ами)")]
    public Vector3 directionTower;
    public int index;
    [Tooltip("Угол поворота башни")]
    public float angleTower;
    [Tooltip("Фактический угол поворота башни")]
    public float currentAngleTower;
    [Tooltip("Может ли башня стрелять")]
    public bool isShot;
    [Tooltip("Столы башни")]
    public List<BaseMuzzle> muzzles;
    [Tooltip("Уровень разрушения")]
    public float levelDestruction;
    public DataTower() {
        muzzles = new();
    }
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