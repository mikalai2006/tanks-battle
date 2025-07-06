using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameBody : ScriptableObject
{
  [Space(5)]
  [Header("Основная информация")]
  public string idObject;
  public TextLocalize text;
  public Sprite spriteBody;

  [Space(5)]
  [Header("Цвета")]
  [Tooltip("Цвет машины")]
  public Color colorBody;

  [Space(5)]
  [Header("Параметры")]
  [Tooltip("Минимальный ранг для доступа")]
  public GameRank minRank;
  [Tooltip("Скорость")]
  [Range(1.5f, 6f)] public float speed;
  [Tooltip("Здоровье")]
  [Range(100f, 1000f)] public int hp;

  [Space(5)]
  [Header("Допустимые улучшения")]
  [Tooltip("Список улучшений")]
  public GameMachine parent;
  public List<MachineUpdateItem> updates;

  // [Tooltip("Задержка выстрела следующего ствола, чтобы имитировать очередь, а не стрелять сразу всеми стволами")]
  // [Range(0f, 0.5f)] public float timeDelayNextMuzzle;
  // [Tooltip("Возможные дулья")]
  // public GameMuzzle Muzzle;
}

