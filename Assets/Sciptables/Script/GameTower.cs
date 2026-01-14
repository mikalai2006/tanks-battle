using System;
using Mikalai2006.Voxel;
using UnityEngine;

[CreateAssetMenu]
public class GameTower : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public TextLocalize text;
  public BaseTower prefab;
  
  [Space(5)]
  [Header("Настройки меша")]
  public MeshConfig MeshConfig;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Минимальный ранг для доступа")]
  public GameRank minRank;
  [Tooltip("Скорость вращения башни")]
  [Range(0f, 720f)] public float speedRotateTower;
  // [Tooltip("Дистанция атаки")]
  // [Range(1f, 15f)] public int distanceAttack;
  // [Tooltip("Дистанция атаки")]
  // [Range(1f, 10f)] public int distanceAttack;
}

