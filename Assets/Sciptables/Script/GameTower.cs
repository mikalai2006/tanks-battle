using System;
using UnityEngine;

[CreateAssetMenu]
public class GameTower : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public Sprite spriteTower;
  public BaseTower prefab;

  // [Space(5)]
  // [Header("Допустимые улучшения")]
  // [Tooltip("Список улучшений")]
  // public GameMachine parent;
  // public List<MachineUpdateItem> updates;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Минимальный ранг для доступа")]
  public GameRank minRank;
  [Tooltip("Скорость вращения башни")]
  [Range(0f, 10f)] public float speedRotateTower;
  [Tooltip("Дистанция обнаружения противника")]
  [Range(1f, 15f)] public int distanceSearch;
  [Tooltip("Дистанция атаки")]
  [Range(1f, 10f)] public int distanceAttack;
}

