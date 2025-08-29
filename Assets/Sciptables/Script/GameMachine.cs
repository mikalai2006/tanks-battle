using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameMachine : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public GameObject machinePrefab;
  public Material material;
  public IndicatorMachine indicatorPrefab;

  [Space(5)]
  [Header("Составные части")]
  public GameBody body;
  public List<GameTowerOption> towers;
  public List<GameCaterpillarOption> catterpillars;

  [Space(5)]
  [Header("Аудио")]
  [Tooltip("Звук мотора")]
  public AudioClip soundMove;

  [Space(5)]
  [Header("Цвета")]
  [Tooltip("Цвет машины")]
  public Color colorBody;
  // [Tooltip("Цвет башни машины")]
  // public Color colorTower; 

  // [Space(5)]
  // [Header("Допустимые улучшения")]
  // [Tooltip("Список улучшений")]
  // public GameMachine parent;
  public List<MachineUpdateItem> updates;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Минимальный ранг для доступа")]
  public GameRank minRank;
  [Tooltip("Скорость")]
  [Range(0.2f, 1000f)] public float speed;
  // [Tooltip("Максимально возможная скорость")]
  // [Range(1.5f, 6f)] public float maxSpeed;
  // [Tooltip("Скорость вращения башни")]
  // [Range(0f, 10f)] public float speedRotateTower;
  // [Tooltip("Максимально возможная скорость вращения башни")]
  // [Range(0f, 10f)] public float maxSpeedRotateTower;
  [Tooltip("Дистанция обнаружения противника")]
  [Range(1f, 15f)] public int distanceSearch;
  // [Tooltip("Дистанция атаки")]
  // [Range(1f, 10f)] public int distanceAttack;
  [Tooltip("Здоровье")]
  [Range(100f, 1000f)] public int hp;
  [Tooltip("Броня")]
  [Range(100f, 1000f)] public int armour;

  // [Tooltip("Задержка выстрела следующего ствола, чтобы имитировать очередь, а не стрелять сразу всеми стволами")]
  // [Range(0f, 0.5f)] public float timeDelayNextMuzzle;
  // [Tooltip("Возможные дулья")]
  // public GameMuzzle Muzzle;
}


[Serializable]
public class GameTowerOption
{
  public string ido;
  // [Tooltip("Спрайт башни")]
  // public Sprite spriteTower;
  [Tooltip("Конфигурация")]
  public GameTower Config;
  [Tooltip("Смещение башни")]
  public Vector3 offsetTower;
  [Tooltip("Цвет башни машины")]
  public Color colorTower;
  [Tooltip("Вращается ли башня")]
  public bool isRotate;

  [Tooltip("Стволы башни")]
  public List<GameMuzzleOption> muzzles;
  public List<string> children;
  public bool isChildren;

  public GameTowerOption()
  {
    children = new();
  }
}

[Serializable]
public class GameCaterpillarOption
{
  [Tooltip("Конфигурация")]
  public GameCaterpillar Config;
  [Tooltip("Смещение")]
  public Vector3 offsetCat;
  [Tooltip("Цвет")]
  public Color colorCat;
}


[Serializable]
public class GameMuzzleOption
{
  [Tooltip("Опции ствола")]
  public GameMuzzle Config;
  public Vector3 offsetMuzzle;
}


[Serializable]
public class MachineUpdateItem
{
  public int level;
  public List<GameUpdate> needUpdates;
  public GameUpdate update;
}